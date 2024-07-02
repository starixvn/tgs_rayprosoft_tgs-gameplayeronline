namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

public record class PlayerOnlineFormGetQuery(
	string GameProviderCode,
	string PlayerCode);
