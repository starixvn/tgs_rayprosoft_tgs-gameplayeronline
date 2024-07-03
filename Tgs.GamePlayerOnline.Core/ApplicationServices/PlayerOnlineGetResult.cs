namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

public sealed record class PlayerOnlineGetResult(
	string GameProviderCode,
	string PlayerCode,
	decimal Balance,
	decimal FreeBalance,
	decimal OutstandingBalance);
