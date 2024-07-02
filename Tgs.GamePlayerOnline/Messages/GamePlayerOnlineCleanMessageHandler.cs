using Tgs.GamePlayerOnline.Core;
using Tgs.GamePlayerOnline.Protos;
using Valhalla.MessageQueue;

namespace Tgs.GamePlayerOnline.Messages;

internal class GamePlayerOnlineCleanMessageHandler(
	ILogger<GamePlayerOnlineCleanMessageHandler> logger,
	IPlayerOnlineCleanDispatcher dispatcher) : IMessageHandler<PlayerOnlineCleanRequest>
{
	public ValueTask HandleAsync(string subject, PlayerOnlineCleanRequest data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("{logInformation}",
			new
			{
				LongOn = DateTime.Now.ToString("o"),
				Subject = subject
			});

		return dispatcher.PushCommandAsync(
			command: new Core.ApplicationServices.PlayerOnlineCleanCommand(
				Deadline: DateTime.Now.Subtract(TimeSpan.FromMinutes(10))),
			cancellationToken: cancellationToken);
	}
}
