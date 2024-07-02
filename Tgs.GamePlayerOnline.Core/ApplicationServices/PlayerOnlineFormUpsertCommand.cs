namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

public record class PlayerOnlineFormUpsertCommand(
	string GameProviderCode,
	string PlayerCode,
	decimal Balance,
	decimal FreeBalance,
	decimal OutstandingBalance);
