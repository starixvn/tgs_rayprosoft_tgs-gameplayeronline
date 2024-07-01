using System.Reflection;
using Tgs.GamePlayerOnline.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGamePlayerOnlineCore(
		this IServiceCollection services,
		Action<IPAddressOptions, IServiceProvider> ipAddressOptions)
		=> services
			.AddOptions<IPAddressOptions>()
			.Configure(ipAddressOptions)
			.Services
			.RegisterAllServiceUtilityTypes(
				targetAssembly: Assembly.GetExecutingAssembly(),
				lifetime: ServiceLifetime.Transient)
			.AddSingleton<ISnowflakeWork, SnowflakeWork>();
}
