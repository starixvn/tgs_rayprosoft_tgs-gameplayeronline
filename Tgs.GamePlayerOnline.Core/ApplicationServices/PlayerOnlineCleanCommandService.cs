using Tgs.ServiceUtility;

namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

internal class PlayerOnlineCleanCommandService(
	IQueryService<PlayerOnlineGetUpdatedBeforeQuery, PlayerOnlineGetResult> queryService,
	ICommandService<PlayerOnlineDeleteCommand> deleteService) : ICommandService<PlayerOnlineCleanCommand>
{
	public async ValueTask ExecuteAsync(PlayerOnlineCleanCommand command, CancellationToken cancellationToken = default)
	{
		await foreach (var result in queryService.QueryAsync(new(command.Deadline), cancellationToken))
		{
			await deleteService.ExecuteAsync(new(result.GameProviderCode, result.PlayerCode), cancellationToken).ConfigureAwait(false);
		}
	}
}
