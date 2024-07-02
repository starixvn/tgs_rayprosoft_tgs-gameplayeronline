using Tgs.ServiceUtility;

namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

internal class PlayerOnlineFormUpsertService(
	IPlayerOnlineRepository playerOnlineRepository)
	: ICommandService<PlayerOnlineFormUpsertCommand>
{
	public async ValueTask ExecuteAsync(PlayerOnlineFormUpsertCommand command, CancellationToken cancellationToken = default)
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
