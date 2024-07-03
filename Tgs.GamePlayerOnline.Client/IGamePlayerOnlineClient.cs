using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.GamePlayerOnline.Client;

public interface IGamePlayerOnlineClient
{
	ValueTask PlayerOnlineUpsertAsync(string gameProviderCode, string playerCode, decimal balance, decimal freeBalance, decimal outstandingBalance, CancellationToken cancellationToken = default);
}
