namespace Tgs.GamePlayerOnline.Core;
public record FundInWhitelistOptions
{
	public IEnumerable<string> ReleaseNamespaces { get; init; } = [];
	public string ReleaseNamespace { get; init; } = default!;
}
