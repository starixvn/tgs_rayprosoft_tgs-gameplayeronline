using Tgs.ServiceUtility;

namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

internal class PlayerOnlineActionCommandService(
	ICommandService<PlayerOnlineUpsertCommand> upsertService,
	ICommandService<PlayerOnlineDeleteCommand> deleteService) : ICommandService<PlayerOnlineActionCommand>
{
	public ValueTask ExecuteAsync(PlayerOnlineActionCommand command, CancellationToken cancellationToken = default)
	{
		return command.Balance >= 0.01m && command.IsUpsert
			? upsertService.ExecuteAsync(
				command: new PlayerOnlineUpsertCommand(
					GameProviderCode: command.GameProviderCode,
					PlayerCode: command.PlayerCode,
					Balance: command.Balance,
					FreeBalance: command.FreeBalance,
					OutstandingBalance: command.OutstandingBalance),
				cancellationToken: cancellationToken)
			: deleteService.ExecuteAsync(
				command: new PlayerOnlineDeleteCommand(
					GameProviderCode: command.GameProviderCode,
					PlayerCode: command.PlayerCode),
				cancellationToken: cancellationToken);
	}
}
