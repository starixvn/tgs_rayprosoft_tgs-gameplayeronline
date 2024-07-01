namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

public sealed record class PlayerOnlineFormGetResult(
	string GameProviderCode,
	string BrandCode,
	string PlayerCode,
	string Username,
	decimal Balance,
	decimal FreeBalance,
	decimal OutstandingBalance);
