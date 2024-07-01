namespace Tgs.GamePlayerOnline.Core.Models;

public sealed record PlayerOnlineForm(
	long Id,
	string GameProviderCode,
	string BrandCode,
	string PlayerCode,
	string Username,
	decimal Balance,
	decimal FreeBalance,
	decimal OutstandingBalance,
	DateTime UpdateOn,
	DateTime? ExpireOn);
