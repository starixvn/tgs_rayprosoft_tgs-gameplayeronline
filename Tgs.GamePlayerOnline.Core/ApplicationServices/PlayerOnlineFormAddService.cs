using Tgs.ServiceUtility;

namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

internal sealed class PlayerOnlineFormAddService(
	IPlayerOnlineRepository playerOnlineRepository)
	: ICommandService<PlayerOnlineFormAddCommand>
{
	public async ValueTask ExecuteAsync(PlayerOnlineFormAddCommand command, CancellationToken cancellationToken = default)
	{
		await playerOnlineRepository
			.AddAsync(
				gameProviderCode: command.GameProviderCode,
				brandCode: command.BrandCode,
				playerCode: command.PlayerCode,
				username: command.Username,
				balance: command.Balance,
				freeBalance: command.FreeBalance,
				outstandingBalance: command.OutstandingBalance,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
