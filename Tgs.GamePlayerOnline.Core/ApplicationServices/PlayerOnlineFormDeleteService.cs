using Tgs.ServiceUtility;

namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

internal sealed class PlayerOnlineFormDeleteService(
	IPlayerOnlineRepository playerOnlineRepository)
	: ICommandService<PlayerOnlineFormDeleteCommand>
{
	public async ValueTask ExecuteAsync(PlayerOnlineFormDeleteCommand command, CancellationToken cancellationToken = default)
	{
		await playerOnlineRepository
			.DeleteAsync(
				gameProviderCode: command.GameProviderCode,
				playerCode: command.PlayerCode,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
