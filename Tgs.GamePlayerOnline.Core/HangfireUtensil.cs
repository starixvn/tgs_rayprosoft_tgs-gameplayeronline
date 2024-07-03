using Tgs.ServiceUtility;
using Tgs.GamePlayerOnline.Core.JobApplicationServices;

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
