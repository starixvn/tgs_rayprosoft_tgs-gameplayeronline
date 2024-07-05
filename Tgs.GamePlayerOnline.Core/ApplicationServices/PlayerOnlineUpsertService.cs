using Tgs.ServiceUtility;

namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

internal sealed class PlayerOnlineUpsertService(
	IPlayerOnlineRepository playerOnlineRepository)
	: ICommandService<PlayerOnlineUpsertCommand>
{
	public async ValueTask ExecuteAsync(PlayerOnlineUpsertCommand command, CancellationToken cancellationToken = default)
	{
		var playerOnline = await playerOnlineRepository.GetAsync(
			gameProviderCode: command.GameProviderCode,
			playerCode: command.PlayerCode,
			cancellationToken).ConfigureAwait(false);

		if (playerOnline is not null
			&& playerOnline.Balance == command.Balance
			&& playerOnline.FreeBalance == command.FreeBalance
			&& playerOnline.OutstandingBalance == command.OutstandingBalance)
		{
			return;
		}

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
