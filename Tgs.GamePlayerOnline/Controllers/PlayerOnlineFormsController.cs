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

	/// <param name="lastUpdateOn">
	///     <example>2024-07-01 17:00:00</example>
	/// </param>
	[HttpGet("GetUpdatedBefore")]
	public async ValueTask<PlayerOnlineFormGetResult[]> GetUpdatedBeforeAsync(
		[FromServices] IQueryService<PlayerOnlineFormGetUpdatedBeforeQuery, PlayerOnlineFormGetResult> service,
		[FromQuery] DateTime lastUpdateOn)
		=> await service.QueryAsync(
			query: new PlayerOnlineFormGetUpdatedBeforeQuery(
				LastUpdateOn: lastUpdateOn),
			cancellationToken: HttpContext.RequestAborted)
			.ToArrayAsync(HttpContext.RequestAborted)
			.ConfigureAwait(false);

	/// <param name="gameProviderCode">
	///     <example>KY</example>
	/// </param>
	/// <param name="playerCode">
	///     <example>4ucmqplxb029</example>
	/// </param>
	[HttpPut("Upsert/{gameProviderCode}/{playerCode}")]
	public ValueTask UpsertAsync(
		[FromServices] ICommandService<PlayerOnlineFormUpsertCommand> service,
		string gameProviderCode,
		string playerCode,
		PlayerOnlineFormViewModel source)
		=> service.ExecuteAsync(
			command: new PlayerOnlineFormUpsertCommand(
				GameProviderCode: gameProviderCode,
				PlayerCode: playerCode,
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
