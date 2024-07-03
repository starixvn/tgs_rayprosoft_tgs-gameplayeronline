
using Microsoft.Extensions.Logging;
using Tgs.GamePlayerOnline.Protos;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GamePlayerOnline.Core.JobApplicationServices;

internal class JobExecuteCommandService(
	ILogger<JobExecuteCommandService> logger,
	IMessageSender service)
	: ICommandService<JobExecuteCommand>
{
	public ValueTask ExecuteAsync(JobExecuteCommand command, CancellationToken cancellationToken = default)
	{
		logger.LogDebug(nameof(ExecuteAsync));
		return service.PublishAsync(
			subject: "game.gameplayeronline.clean.request",
			data: new PlayerOnlineCleanRequest(),
			cancellationToken: cancellationToken
		);
	}
}
