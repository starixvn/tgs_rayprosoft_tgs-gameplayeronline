using Tgs.ServiceUtility;

namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

internal sealed class PlayerOnlineDeleteService(
	IPlayerOnlineRepository playerOnlineRepository)
	: ICommandService<PlayerOnlineDeleteCommand>
{
	public async ValueTask ExecuteAsync(PlayerOnlineDeleteCommand command, CancellationToken cancellationToken = default)
	{
		await playerOnlineRepository
			.DeleteAsync(
				gameProviderCode: command.GameProviderCode,
				playerCode: command.PlayerCode,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
