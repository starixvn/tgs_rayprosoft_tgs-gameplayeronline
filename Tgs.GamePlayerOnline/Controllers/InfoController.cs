using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Net;
using System.Reflection;
using Microsoft.Extensions.Options;
using Tgs.GamePlayerOnline.Core;

namespace Tgs.GamePlayerOnline.Controllers;

[Route("[controller]")]
[ApiController]
public class InfoController : ControllerBase
{
	/// <summary>
	/// Gets the asynchronous.
	/// </summary>
	/// <param name="webHostEnvironment">The web host environment.</param>
	/// <param name="configuration">The configuration.</param>
	/// <param name="fundInWhitelistOptions">The fund in whitelist options.</param>
	/// <returns></returns>
	[HttpGet]
	public async Task<object> GetAsync(
		[FromServices] IWebHostEnvironment webHostEnvironment,
		[FromServices] IConfiguration configuration,
		[FromServices] IOptions<FundInWhitelistOptions> fundInWhitelistOptions)
	{
		var hostName = Dns.GetHostName();
		var hostEntry = await Dns.GetHostEntryAsync(hostName).ConfigureAwait(false);
		var hostIp = Array.Find(hostEntry.AddressList,
			x => x.AddressFamily == AddressFamily.InterNetwork);

		return new
		{
			Environment.MachineName,
			HostName = hostName,
			HostIp = hostIp?.ToString() ?? string.Empty,
			Environment = webHostEnvironment.EnvironmentName,
			OsVersion = $"{Environment.OSVersion}",
			Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
			ProcessCount = Environment.ProcessorCount,
			GamePlayerOnlineConnectionStrings = configuration.GetSection("GamePlayerOnlineConnectionStrings").Get<Dictionary<string, string>>(),
			FundInWhitelistOptions = fundInWhitelistOptions.Value
		};
	}
}
