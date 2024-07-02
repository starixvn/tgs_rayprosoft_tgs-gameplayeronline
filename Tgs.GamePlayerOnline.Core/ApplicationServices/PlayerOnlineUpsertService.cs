using Tgs.ServiceUtility;

namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

internal sealed class PlayerOnlineUpsertService(
	IPlayerOnlineRepository playerOnlineRepository)
	: ICommandService<PlayerOnlineUpsertCommand>
{
	public async ValueTask ExecuteAsync(PlayerOnlineUpsertCommand command, CancellationToken cancellationToken = default)
	{
		await playerOnlineRepository
			.UpsertAsync(
				gameProviderCode: command.GameProviderCode,
				playerCode: command.PlayerCode,
				balance: command.Balance,
				freeBalance: command.FreeBalance,
				outstandingBalance: command.OutstandingBalance,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
