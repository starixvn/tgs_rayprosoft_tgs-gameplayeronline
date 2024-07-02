using System.Runtime.CompilerServices;
using Tgs.ServiceUtility;

namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

internal class PlayerOnlineFormGetUpdatedBeforeService(
	IPlayerOnlineRepository playerOnlineRepository)
	: IQueryService<PlayerOnlineFormGetUpdatedBeforeQuery, PlayerOnlineFormGetResult>
{
	public async IAsyncEnumerable<PlayerOnlineFormGetResult> QueryAsync(PlayerOnlineFormGetUpdatedBeforeQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var result = playerOnlineRepository
			.GetUpdatedBeforeAsync(
				lastUpdateOn: query.LastUpdateOn,
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
