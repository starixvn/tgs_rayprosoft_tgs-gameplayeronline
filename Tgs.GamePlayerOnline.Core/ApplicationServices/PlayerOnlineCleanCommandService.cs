using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.ServiceUtility;

namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

internal class PlayerOnlineCleanCommandService(
	ILogger<PlayerOnlineCleanCommandService> logger,
	IOptions<FundInWhitelistOptions> fundInWhitelistOptions,
	IQueryService<PlayerOnlineGetUpdatedBeforeQuery, PlayerOnlineGetResult> queryService,
	ICommandService<PlayerOnlineCheckingPlayerCommand> checkingPlayerService) : ICommandService<PlayerOnlineCleanCommand>
{
	private readonly FundInWhitelistOptions _fundInWhitelistOptions = fundInWhitelistOptions.Value;

	public async ValueTask ExecuteAsync(PlayerOnlineCleanCommand command, CancellationToken cancellationToken = default)
	{
		if (!_fundInWhitelistOptions.ReleaseNamespaces.Contains(_fundInWhitelistOptions.ReleaseNamespace))
		{
			logger.LogWarning("{logWarning}",
				new
				{
					LongOn = DateTime.Now.ToString("o"),
					Message = $"玩家自動轉入功能不在此ReleaseNamespace({_fundInWhitelistOptions.ReleaseNamespace})，目前開放名單:{string.Join(",", _fundInWhitelistOptions.ReleaseNamespaces)}"
				});
			return;
		}

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
