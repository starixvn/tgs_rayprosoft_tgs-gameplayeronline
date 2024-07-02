namespace Tgs.GamePlayerOnline.Core.Models;

public record PlayerOnlineForm(
	string GameProviderCode,
	string BrandCode,
	string PlayerCode,
	string Username,
	decimal Balance,
	decimal FreeBalance,
	decimal OutstandingBalance,
	DateTime UpdateOn,
	DateTime? ExpireOn);
