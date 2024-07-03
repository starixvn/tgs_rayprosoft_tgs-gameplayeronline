using System.Runtime.CompilerServices;
using Tgs.ServiceUtility;

namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

internal sealed class PlayerOnlineGetUpdatedBeforeService(
	IPlayerOnlineRepository playerOnlineRepository)
	: IQueryService<PlayerOnlineGetUpdatedBeforeQuery, PlayerOnlineGetResult>
{
	public async IAsyncEnumerable<PlayerOnlineGetResult> QueryAsync(PlayerOnlineGetUpdatedBeforeQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var result = playerOnlineRepository
			.GetUpdatedBeforeAsync(
				lastUpdateOn: query.LastUpdateOn,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await foreach (var data in result)
		{
			yield return new PlayerOnlineGetResult(
				GameProviderCode: data.GameProviderCode,
				PlayerCode: data.PlayerCode,
				Balance: data.Balance,
				FreeBalance: data.FreeBalance,
				OutstandingBalance: data.OutstandingBalance);
		}
	}
}
