namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

public record class PlayerOnlineFormDeleteCommand(
	string GameProviderCode,
	string PlayerCode);
