using Tgs.ServiceUtility;

namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

internal sealed class PlayerOnlineFormUpdateService(
	IPlayerOnlineRepository playerOnlineRepository)
	: ICommandService<PlayerOnlineFormUpdateCommand>
{
	public async ValueTask ExecuteAsync(PlayerOnlineFormUpdateCommand command, CancellationToken cancellationToken = default)
	{
		await playerOnlineRepository
			.UpdateAsync(
				gameProviderCode: command.GameProviderCode,
				playerCode: command.PlayerCode,
				balance: command.Balance,
				freeBalance: command.FreeBalance,
				outstandingBalance: command.OutstandingBalance,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
