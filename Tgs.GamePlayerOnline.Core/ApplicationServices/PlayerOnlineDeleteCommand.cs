namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

public sealed record class PlayerOnlineDeleteCommand(
	string GameProviderCode,
	string PlayerCode);
