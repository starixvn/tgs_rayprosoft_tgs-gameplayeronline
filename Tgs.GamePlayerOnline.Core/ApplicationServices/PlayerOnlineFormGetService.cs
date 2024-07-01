using Tgs.ServiceUtility;

namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

internal sealed class PlayerOnlineFormGetService(
	IPlayerOnlineRepository playerOnlineRepository)
	: IGetService<PlayerOnlineFormGetQuery, PlayerOnlineFormGetResult?>
{
	public async ValueTask<PlayerOnlineFormGetResult?> GetAsync(PlayerOnlineFormGetQuery query, CancellationToken cancellationToken = default)
	{
		var result = await playerOnlineRepository
			.GetAsync(
				gameProviderCode: query.GameProviderCode,
				playerCode: query.PlayerCode,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		
		return result == null ? null : new PlayerOnlineFormGetResult(
			GameProviderCode: result.GameProviderCode,
			BrandCode: result.BrandCode,
			PlayerCode: result.PlayerCode,
			Username: result.Username,
			Balance: result.Balance,
			FreeBalance: result.FreeBalance,
			OutstandingBalance: result.OutstandingBalance);
	}
}
