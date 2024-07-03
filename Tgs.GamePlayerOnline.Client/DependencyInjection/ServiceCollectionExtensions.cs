using Microsoft.Extensions.DependencyInjection;

namespace Tgs.GamePlayerOnline.Client.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGamePlayerOnlineClient(this IServiceCollection services, Action<IHttpClientBuilder> httpClientConfigure)
	{
		httpClientConfigure(services.AddHttpClient<IGamePlayerOnlineClient, GamePlayerOnlineClient>());

		return services;
	}
}
