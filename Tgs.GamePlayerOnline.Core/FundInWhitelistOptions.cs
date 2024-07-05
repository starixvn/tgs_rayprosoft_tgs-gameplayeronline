namespace Tgs.GamePlayerOnline.Core;
public record FundInWhitelistOptions
{
	public IEnumerable<string> ReleaseNamespaces { get; set; } = [];
	public string ReleaseNamespace { get; set; } = default!;
}
