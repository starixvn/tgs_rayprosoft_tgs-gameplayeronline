namespace Tgs.GamePlayerOnline.Core.ApplicationServices;
public record PlayerOnlineCheckingPlayerCommand(
	string GameProviderCode,
	string PlayerCode,
	decimal Balance,
	decimal FreeBalance,
	decimal OutstandingBalance);
