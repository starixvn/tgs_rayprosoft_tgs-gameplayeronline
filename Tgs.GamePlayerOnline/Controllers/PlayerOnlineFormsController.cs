using Microsoft.AspNetCore.Mvc;
using Tgs.GamePlayerOnline.Core.ApplicationServices;
using Tgs.GamePlayerOnline.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GamePlayerOnline.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayerOnlineFormsController : ControllerBase
{
	/// <param name="gameProviderCode">
	///     <example>KY</example>
	/// </param>
	/// <param name="playerCode">
	///     <example>4ucmqplxb029</example>
	/// </param>
	[HttpGet("Get/{gameProviderCode}/{playerCode}")]
	public ValueTask<PlayerOnlineFormGetResult> GetAsync(
		[FromServices] IGetService<PlayerOnlineFormGetQuery, PlayerOnlineFormGetResult> service,
		string gameProviderCode,
		string playerCode)
		=> service.GetAsync(
			query: new PlayerOnlineFormGetQuery(
				GameProviderCode: gameProviderCode,
				PlayerCode: playerCode),
			cancellationToken: HttpContext.RequestAborted);

	/// <param name="gameProviderCode">
	///     <example>KY</example>
	/// </param>
	/// <param name="playerCode">
	///     <example>4ucmqplxb029</example>
	/// </param>
	[HttpPost("Add/{gameProviderCode}/{playerCode}")]
	public ValueTask AddAsync(
		[FromServices] ICommandService<PlayerOnlineFormAddCommand> service,
		string gameProviderCode,
		string playerCode,
		PlayerOnlineFormAddViewModel source)
		=> service.ExecuteAsync(
			command: new PlayerOnlineFormAddCommand(
				GameProviderCode: gameProviderCode,
				BrandCode: "GPI",
				PlayerCode: playerCode,
				Username: playerCode,
				Balance: source.Balance,
				FreeBalance: source.FreeBalance,
				OutstandingBalance: source.OutstandingBalance),
			cancellationToken: HttpContext.RequestAborted);

	/// <param name="gameProviderCode">
	///     <example>KY</example>
	/// </param>
	/// <param name="playerCode">
	///     <example>4ucmqplxb029</example>
	/// </param>
	[HttpDelete("Delete/{gameProviderCode}/{playerCode}")]
	public ValueTask DeleteAsync(
		[FromServices] ICommandService<PlayerOnlineFormDeleteCommand> service,
		string gameProviderCode,
		string playerCode)
		=> service.ExecuteAsync(
			command: new PlayerOnlineFormDeleteCommand(
				GameProviderCode: gameProviderCode,
				PlayerCode: playerCode),
			cancellationToken: HttpContext.RequestAborted);
}
