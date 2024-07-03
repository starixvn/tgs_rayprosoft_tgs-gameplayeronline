using Tgs.GamePlayerOnline.Core.JobApplicationServices;

using Tgs.ServiceUtility;

namespace Tgs.GamePlayerOnline.Core;

internal class HangfireUtensil(
	ICommandService<JobInitCommand> jobInitservice,
	ICommandService<JobExecuteCommand> jobExecuteService)
	: IHangfireUtensil
{
	public Task JobInitAsync()
		=> jobInitservice.ExecuteAsync(new()).AsTask();

	public Task JobExecuteAsync(JobExecuteCommand source)
		=> jobExecuteService.ExecuteAsync(source).AsTask();
}
