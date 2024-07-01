using Tgs.GamePlayerOnline.Core.Models;

namespace Tgs.GamePlayerOnline.Core;

public interface IPlayerOnlineRepository
{
	ValueTask InitializeAsync(CancellationToken cancellationToken = default);

	ValueTask<PlayerOnlineForm?> GetAsync(string gameProviderCode, string playerCode, CancellationToken cancellationToken = default);

	ValueTask<long> AddAsync(string gameProviderCode, string brandCode, string playerCode, string username, decimal balance, decimal freeBalance, decimal outstandingBalance, CancellationToken cancellationToken = default);

	ValueTask<long> DeleteAsync(string gameProviderCode, string playerCode, CancellationToken cancellationToken = default);
}
