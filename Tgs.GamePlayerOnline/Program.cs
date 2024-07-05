using System.Net.Mime;
using System.Text.Json.Serialization;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.Redis.StackExchange;
using Microsoft.AspNetCore.Mvc;
using NATS.Client.Core;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Tgs.GamePlayerOnline;
using Tgs.GamePlayerOnline.Core;
using Tgs.GamePlayerOnline.Messages;
using Tgs.MessageQueue.Nats.Serializers;

var filterInstrumentations = new[] { "html", "ico", "png", "jpg" };

var builder = WebApplication.CreateBuilder(args);

builder.Logging
	.AddOpenTelemetry(options =>
	{
		options.IncludeFormattedMessage = true;
		options.ParseStateValues = true;
		_ = options.AddFluentdForwardExporter(fluentOptions =>
		{
			var nodeIp = builder.Configuration.GetValue<string>("NODE_IP")!;
			fluentOptions.Host = nodeIp;
			fluentOptions.Tag = builder.Configuration.GetValue<string>("OTEL_SERVICE_NAME")!;
			fluentOptions.UseMessagePack();
		});
	});

// Add services to the container.

builder.Services
	.AddRouting(options => options.LowercaseUrls = true)
	.AddControllers(options =>
	{
		options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
		options.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));
	})
	.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	foreach (var xmlFileName in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml"))
	{
		c.IncludeXmlComments(xmlFileName);
	}
});

builder.Services.AddHangfire(
	(sp, config) =>
	{
		var env = sp.GetRequiredService<IWebHostEnvironment>();

		if (!env.EnvironmentName.EndsWith("test", StringComparison.OrdinalIgnoreCase))
		{
			var connectionString = builder.Configuration.GetConnectionString("redis")!;
			_ = config
				.UseRedisStorage(connectionString, new() { Prefix = "game-playeronline-hangfire:" })
				.WithJobExpirationTimeout(TimeSpan.FromMinutes(30));
		}
		else
		{
			_ = config.UseMemoryStorage();
		}
	})
	.AddHangfireServer();

builder.Services
	.AddOptions<FundInWhitelistOptions>()
	.Configure((options) =>
	{
		builder.Configuration.GetSection("FundInWhitelistOptions").Bind(options);
		options.ReleaseNamespace = builder.Configuration.GetValue<string>("RELEASE_NAMESPACE")!;
	})
	.Services
	.AddGamePlayerOnline(
	(options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.ConnectionString = configuration.GetConnectionString("Mongo")!;
	})
	.AddGameProvidersClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GamePlayerOnlineConnectionStrings:GameProvidersClientBaseUrl")!;
	})
	.AddGameProvidersPluggableServerClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GamePlayerOnlineConnectionStrings:GameProvidersPluggableClientBaseUrl")!;
	});

builder.Services
	.AddKeyedSingleton("Nats", (sp, _) => new NatsConnection(NatsOpts.Default with
	{
		Url = builder.Configuration.GetConnectionString("Nats")!,
		SerializerRegistry = TgsNatsSerializerRegistration.Default
	}))
	.AddMessageQueue()
	.AddNatsMessageQueue(config => config
	.ConfigureResolveConnection(sp => sp.GetRequiredKeyedService<NatsConnection>("Nats"))
	.AddHandler<GamePlayerOnlineUpsertMessageHandler>("game.gameplayeronline.upsert.request", typeof(GamePlayerOnlineUpsertMessageHandler).FullName!.ToLower())
	.AddHandler<GamePlayerOnlineDeleteMessageHandler>("game.gameplayeronline.delete.request", typeof(GamePlayerOnlineDeleteMessageHandler).FullName!.ToLower())
	.AddHandler<GamePlayerOnlineCleanMessageHandler>("game.gameplayeronline.clean.request", typeof(GamePlayerOnlineCleanMessageHandler).FullName!.ToLower()))
	.ClearAllExchanges()
	.AddNatsGlobPatternExchange("game.gameplayeronline.upsert.request")
	.AddNatsGlobPatternExchange("game.gameplayeronline.delete.request")
	.AddNatsGlobPatternExchange("game.gameplayeronline.clean.request");

builder.Services
	.AddOpenTelemetry()
	.WithTracing(builder => builder
		.SetResourceBuilder(ResourceBuilder.CreateDefault()
			.AddEnvironmentVariableDetector())
		.AddSource("Tgs.*")
		.AddSource("TGS.*")
		.AddSource("Valhalla.*")
		.AddAspNetCoreInstrumentation(options => options.Filter =
			httpContext => !httpContext.Request.Path.HasValue
			|| !httpContext.Request.Path.StartsWithSegments("/swagger")
			&& !httpContext.Request.Path.StartsWithSegments("/healthz")
			&& !httpContext.Request.Path.StartsWithSegments("/profiler/")
			&& !httpContext.Request.Path.StartsWithSegments("/hangfire")
			&& !httpContext.Request.Path.StartsWithSegments("/metrics")
			&& !filterInstrumentations.Any(ext => httpContext.Request.Path.Value.EndsWith($".{ext}")))
		.AddHttpClientInstrumentation()
		.AddGrpcClientInstrumentation()
		.AddGrpcCoreInstrumentation()
		.AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
		.AddOtlpExporter())
	.WithMetrics(meterBuiler => meterBuiler
	.SetResourceBuilder(ResourceBuilder.CreateDefault()
		.AddEnvironmentVariableDetector())
		.AddRuntimeInstrumentation()
		.AddAspNetCoreInstrumentation()
		.AddOtlpExporter())
	.Services
	.AddInterceptionService()
	.AddServiceUtiltiyInterceptors();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
_ = app.UseSwagger();
_ = app.UseSwaggerUI();

app.UseHangfireDashboard(
	"/hangfire",
	new DashboardOptions
	{
		Authorization = [
			new IgnoreHangfireAuthorizationFilter()
		]
	});

_ = app.MapControllers();

app.MapHealthChecks("/healthz");

_ = app.UseHangfireRegisterAsync();

app.Run();
