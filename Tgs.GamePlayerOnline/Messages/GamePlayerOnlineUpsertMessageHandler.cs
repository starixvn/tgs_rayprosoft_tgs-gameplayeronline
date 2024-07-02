using Tgs.GamePlayerOnline.Core;
using Tgs.GamePlayerOnline.Protos;
using Valhalla.MessageQueue;

namespace Tgs.GamePlayerOnline.Messages;

internal class GamePlayerOnlineUpsertMessageHandler(
	ILogger<GamePlayerOnlineUpsertMessageHandler> logger,
	IPlayerOnlineActionDispatcher dispatcher) : IMessageHandler<PlayerOnlineUpsertRequest>
{
	public ValueTask HandleAsync(string subject, PlayerOnlineUpsertRequest data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("{logInformation}",
			new
			{
				LongOn = DateTime.Now.ToString("o"),
				Subject = subject,
				Data = new
				{
					data.GameProviderCode,
					data.PlayerCode,
					Balance = data.Balance.ToDecimal().GetValueOrDefault(0),
					FreeBalance = data.FreeBalance.ToDecimal().GetValueOrDefault(0),
					OutstandingBalance = data.OutstandingBalance.ToDecimal().GetValueOrDefault(0)
				}
			});

		return dispatcher.PushCommandAsync(
			command: new Core.ApplicationServices.PlayerOnlineActionCommand(
				GameProviderCode: data.GameProviderCode,
				PlayerCode: data.PlayerCode,
				IsUpsert: true,
				Balance: data.Balance.ToDecimal().GetValueOrDefault(0),
				FreeBalance: data.FreeBalance.ToDecimal().GetValueOrDefault(0),
				OutstandingBalance: data.OutstandingBalance.ToDecimal().GetValueOrDefault(0)),
			cancellationToken: cancellationToken);
	}
}
