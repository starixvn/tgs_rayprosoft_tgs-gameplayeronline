namespace Tgs.GamePlayerOnline.Core.Models;

public sealed record PlayerOnlineForm(
	string GameProviderCode,
	string PlayerCode,
	decimal Balance,
	decimal FreeBalance,
	decimal OutstandingBalance,
	DateTime UpdateOn,
	DateTime? ExpireOn);
