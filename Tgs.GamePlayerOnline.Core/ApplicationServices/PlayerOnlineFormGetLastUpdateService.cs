using System.Runtime.CompilerServices;
using Tgs.ServiceUtility;

namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

internal sealed class PlayerOnlineFormGetLastUpdateService(
	IPlayerOnlineRepository playerOnlineRepository)
	: IQueryService<PlayerOnlineFormGetLastUpdateQuery, PlayerOnlineFormGetResult>
{
	public async IAsyncEnumerable<PlayerOnlineFormGetResult> QueryAsync(PlayerOnlineFormGetLastUpdateQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var result = playerOnlineRepository
			.GetLastUpdateAsync(
				beginUpdateOn: query.BeginUpdateOn,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await foreach (var data in result)
		{
			yield return new PlayerOnlineFormGetResult(
				GameProviderCode: data.GameProviderCode,
				BrandCode: data.BrandCode,
				PlayerCode: data.PlayerCode,
				Username: data.Username,
				Balance: data.Balance,
				FreeBalance: data.FreeBalance,
				OutstandingBalance: data.OutstandingBalance);
		}
	}
}
