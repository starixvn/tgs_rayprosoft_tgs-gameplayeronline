
using Microsoft.Extensions.Logging;
using Tgs.GamePlayerOnline.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GamePlayerOnline.Core.JobApplicationServices;

internal class JobExecuteCommandService(
	ILogger<JobExecuteCommandService> logger,
	ICommandService<PlayerOnlineCleanCommand> service)
	: ICommandService<JobExecuteCommand>
{
	public ValueTask ExecuteAsync(JobExecuteCommand command, CancellationToken cancellationToken = default)
	{
		logger.LogDebug(nameof(ExecuteAsync));
		return service.ExecuteAsync(new(DateTime.Now.Subtract(TimeSpan.FromMinutes(10))), cancellationToken);
	}
}
