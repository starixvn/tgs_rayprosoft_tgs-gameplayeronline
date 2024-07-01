using Tgs.GamePlayerOnline;
using Tgs.GamePlayerOnline.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGamePlayerOnline(
		this IServiceCollection services,
		Action<MongoDBOptions, IServiceProvider> mongoDBconfigure,
		Action<IPAddressOptions, IServiceProvider> ipAddressOptions)
		=> services
			.AddOptions<MongoDBOptions>()
			.Configure(mongoDBconfigure)
			.Services
			.AddSingleton<MongoDBConnectionManager>()
			.AddMemoryCache()
			.AddGamePlayerOnlineCore(ipAddressOptions)
			.AddGamePlayerOnlineRepositories(provider => provider.GetRequiredService<MongoDBConnectionManager>().MongoDatabase);
}
