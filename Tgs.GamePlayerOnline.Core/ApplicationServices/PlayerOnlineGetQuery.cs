namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

public sealed record class PlayerOnlineGetQuery(
	string GameProviderCode,
	string PlayerCode);
