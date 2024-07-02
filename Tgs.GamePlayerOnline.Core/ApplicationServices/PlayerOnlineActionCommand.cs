namespace Tgs.GamePlayerOnline.Core.ApplicationServices;
public record PlayerOnlineActionCommand(
	string GameProviderCode,
	string PlayerCode,
	bool IsUpsert,
	decimal Balance,
	decimal FreeBalance,
	decimal OutstandingBalance);
