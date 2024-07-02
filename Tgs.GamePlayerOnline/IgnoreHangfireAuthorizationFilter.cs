using Hangfire.Dashboard;

namespace Tgs.GamePlayerOnline;

public class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
	public bool Authorize(DashboardContext context) => true;
}
