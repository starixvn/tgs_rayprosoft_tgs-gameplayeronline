using Tgs.GamePlayerOnline.Core.Models;

namespace Tgs.GamePlayerOnline.Core;

public interface IPlayerOnlineRepository
{
	ValueTask InitializeAsync(CancellationToken cancellationToken = default);

	ValueTask<PlayerOnline?> GetAsync(string gameProviderCode, string playerCode, CancellationToken cancellationToken = default);

	IAsyncEnumerable<PlayerOnline> GetUpdatedBeforeAsync(DateTime lastUpdateOn, CancellationToken cancellationToken = default);

	ValueTask UpsertAsync(string gameProviderCode, string playerCode, decimal balance, decimal freeBalance, decimal outstandingBalance, CancellationToken cancellationToken = default);

	ValueTask DeleteAsync(string gameProviderCode, string playerCode, CancellationToken cancellationToken = default);
}
