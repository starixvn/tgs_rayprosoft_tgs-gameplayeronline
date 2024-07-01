namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

public sealed record class PlayerOnlineFormDeleteCommand(
	string GameProviderCode,
	string PlayerCode);
