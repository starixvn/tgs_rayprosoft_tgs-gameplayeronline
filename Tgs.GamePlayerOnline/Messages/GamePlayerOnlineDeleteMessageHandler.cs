using Tgs.GamePlayerOnline.Core;
using Tgs.GamePlayerOnline.Protos;
using Valhalla.MessageQueue;

namespace Tgs.GamePlayerOnline.Messages;

internal class GamePlayerOnlineDeleteMessageHandler(
	ILogger<GamePlayerOnlineDeleteMessageHandler> logger,
	IPlayerOnlineActionDispatcher dispatcher) : IMessageHandler<PlayerOnlineDeleteRequest>
{
	public ValueTask HandleAsync(string subject, PlayerOnlineDeleteRequest data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("{logInformation}",
			new
			{
				LongOn = DateTime.Now.ToString("o"),
				Subject = subject,
				Data = new
				{
					data.GameProviderCode,
					data.PlayerCode
				}
			});

		return dispatcher.PushCommandAsync(
			command: new Core.ApplicationServices.PlayerOnlineActionCommand(
				GameProviderCode: data.GameProviderCode,
				PlayerCode: data.PlayerCode,
				IsUpsert: false,
				Balance: 0.0m,
				FreeBalance: 0.0m,
				OutstandingBalance: 0.0m),
			cancellationToken: cancellationToken);
	}
}
