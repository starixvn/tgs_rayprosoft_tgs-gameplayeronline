namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

public sealed record class PlayerOnlineUpsertCommand(
	string GameProviderCode,
	string PlayerCode,
	decimal Balance,
	decimal FreeBalance,
	decimal OutstandingBalance);
