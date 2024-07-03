using Tgs.GamePlayerOnline.Core.JobApplicationServices;

namespace Tgs.GamePlayerOnline.Core;

public interface IHangfireUtensil
{
	public Task JobInitAsync();

	public Task JobExecuteAsync(JobExecuteCommand source);
}
