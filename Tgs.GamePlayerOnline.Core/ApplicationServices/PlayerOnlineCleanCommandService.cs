using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

internal class PlayerOnlineCleanCommandService(
	ILogger<PlayerOnlineCleanCommandService> logger,
	IQueryService<PlayerOnlineGetUpdatedBeforeQuery, PlayerOnlineGetResult> queryService,
	ICommandService<PlayerOnlineCheckingPlayerCommand> checkingPlayerService) : ICommandService<PlayerOnlineCleanCommand>
{
	public async ValueTask ExecuteAsync(PlayerOnlineCleanCommand command, CancellationToken cancellationToken = default)
	{
		await foreach (var playerOnline in queryService.QueryAsync(new(command.Deadline), cancellationToken))
		{
			try
			{
				await checkingPlayerService.ExecuteAsync(
					command: new PlayerOnlineCheckingPlayerCommand(
						GameProviderCode: playerOnline.GameProviderCode,
						PlayerCode: playerOnline.PlayerCode,
						Balance: playerOnline.Balance,
						FreeBalance: playerOnline.FreeBalance,
						OutstandingBalance: playerOnline.OutstandingBalance),
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "{ logError}",
				new
				{
					LongOn = DateTime.Now.ToString("o"),
					Data = new
					{
						playerOnline.GameProviderCode,
						playerOnline.PlayerCode,
						playerOnline.Balance,
						playerOnline.FreeBalance,
						playerOnline.OutstandingBalance,
					}
				});
			}
		}
	}
}
