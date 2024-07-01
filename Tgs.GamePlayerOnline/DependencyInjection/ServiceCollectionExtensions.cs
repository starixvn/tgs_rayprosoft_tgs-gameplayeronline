using Tgs.GamePlayerOnline;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGamePlayerOnline(
		this IServiceCollection services,
		Action<MongoDBOptions, IServiceProvider> mongoDBconfigure)
		=> services
			.AddOptions<MongoDBOptions>()
			.Configure(mongoDBconfigure)
			.Services
			.AddSingleton<MongoDBConnectionManager>()
			.AddMemoryCache()
			.AddGamePlayerOnlineCore()
			.AddGamePlayerOnlineRepositories(provider => provider.GetRequiredService<MongoDBConnectionManager>().MongoDatabase);
}
