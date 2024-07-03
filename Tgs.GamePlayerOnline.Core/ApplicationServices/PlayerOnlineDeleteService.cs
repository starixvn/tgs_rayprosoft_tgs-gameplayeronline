using Tgs.ServiceUtility;

namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

internal sealed class PlayerOnlineDeleteService(
	IPlayerOnlineRepository playerOnlineRepository)
	: ICommandService<PlayerOnlineDeleteCommand>
{
	public async ValueTask ExecuteAsync(PlayerOnlineDeleteCommand command, CancellationToken cancellationToken = default)
	{
		if (await playerOnlineRepository.GetAsync(
			gameProviderCode: command.GameProviderCode,
			playerCode: command.PlayerCode,
			cancellationToken).ConfigureAwait(false) is not null)
		{
			await playerOnlineRepository.DeleteAsync(
				gameProviderCode: command.GameProviderCode,
				playerCode: command.PlayerCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
	}
}
