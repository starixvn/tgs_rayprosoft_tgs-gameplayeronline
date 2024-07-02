using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGamePlayerOnlineCore(this IServiceCollection services)
		=> services
			.RegisterAllServiceUtilityTypes(
				targetAssembly: Assembly.GetExecutingAssembly(),
				lifetime: ServiceLifetime.Transient);
}
