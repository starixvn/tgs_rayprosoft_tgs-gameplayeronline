namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

public sealed record class PlayerOnlineFormGetQuery(
	string GameProviderCode,
	string PlayerCode);
