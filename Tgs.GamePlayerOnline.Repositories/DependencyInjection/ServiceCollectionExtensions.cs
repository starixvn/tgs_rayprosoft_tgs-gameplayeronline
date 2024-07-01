using MongoDB.Driver;
using Tgs.GamePlayerOnline.Core;
using Tgs.GamePlayerOnline.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGamePlayerOnlineRepositories(
		this IServiceCollection services,
		Func<IServiceProvider, IMongoDatabase> mongoDatabaseResolver)
		=> services
		.AddTransient<IPlayerOnlineRepository>(provider => ActivatorUtilities.CreateInstance<PlayerOnlineRepository>(provider, mongoDatabaseResolver(provider)));
}
