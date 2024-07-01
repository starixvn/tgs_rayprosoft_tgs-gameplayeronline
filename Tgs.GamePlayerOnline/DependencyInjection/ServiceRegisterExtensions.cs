using Tgs.GamePlayerOnline.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceRegisterExtensions
{
	public static async Task<WebApplication> InitializePlayerOnlineFormAsync(this WebApplication app)
	{
		using (var scope = app.Services.CreateScope())
		{
			var repository = scope.ServiceProvider.GetRequiredService<IPlayerOnlineRepository>();
			await repository
				 .InitializeAsync()
				 .ConfigureAwait(false);
		}

		return app;
	}

	public static WebApplication InitialSnowflakeWork(this WebApplication app)
	{
		using (var scope = app.Services.CreateScope())
		{
			var snowflakeWork = scope.ServiceProvider.GetRequiredService<ISnowflakeWork>();
			_ = snowflakeWork.GetId();
		}

		return app;
	}
}
