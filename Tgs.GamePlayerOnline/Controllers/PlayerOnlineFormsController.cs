using Microsoft.AspNetCore.Mvc;
using Tgs.GamePlayerOnline.Core.ApplicationServices;
using Tgs.GamePlayerOnline.Protos;
using Tgs.GamePlayerOnline.ViewModels;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

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
	public ValueTask<PlayerOnlineGetResult> GetAsync(
		[FromServices] IGetService<PlayerOnlineGetQuery, PlayerOnlineGetResult> service,
		string gameProviderCode,
		string playerCode)
		=> service.GetAsync(
			query: new PlayerOnlineGetQuery(
				GameProviderCode: gameProviderCode,
				PlayerCode: playerCode),
			cancellationToken: HttpContext.RequestAborted);

	/// <param name="lastUpdateOn">
	///     <example>2024-07-01 17:00:00</example>
	/// </param>
	[HttpGet("GetUpdatedBefore")]
	public async ValueTask<PlayerOnlineGetResult[]> GetUpdatedBeforeAsync(
		[FromServices] IQueryService<PlayerOnlineGetUpdatedBeforeQuery, PlayerOnlineGetResult> service,
		[FromQuery] DateTime lastUpdateOn)
		=> await service.QueryAsync(
			query: new PlayerOnlineGetUpdatedBeforeQuery(
				LastUpdateOn: lastUpdateOn),
			cancellationToken: HttpContext.RequestAborted)
			.ToArrayAsync(HttpContext.RequestAborted)
			.ConfigureAwait(false);

	/// <summary>
	/// Upserts the asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="gameProviderCode">The game provider code.</param>
	/// <param name="playerCode">The player code.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPut("{gameProviderCode}/{playerCode}")]
	public ValueTask UpsertAsync(
		[FromServices] IMessageSender messageSender,
		string gameProviderCode,
		string playerCode,
		PlayerOnlineFormViewModel source)
		=> messageSender.PublishAsync(
			subject: "game.gameplayeronline.upsert.request",
			data: new PlayerOnlineUpsertRequest
			{
				GameProviderCode = gameProviderCode,
				PlayerCode = playerCode,
				Balance = source.Balance,
				FreeBalance = source.FreeBalance,
				OutstandingBalance = source.OutstandingBalance
			},
			cancellationToken: HttpContext.RequestAborted);

	/// <param name="gameProviderCode">
	///     <example>KY</example>
	/// </param>
	/// <param name="playerCode">
	///     <example>4ucmqplxb029</example>
	/// </param>
	[HttpDelete("Delete/{gameProviderCode}/{playerCode}")]
	public ValueTask DeleteAsync(
		[FromServices] IMessageSender messageSender,
		string gameProviderCode,
		string playerCode)
		=> messageSender.PublishAsync(
			subject: "game.gameplayeronline.delete.request",
			data: new PlayerOnlineDeleteRequest
			{
				GameProviderCode = gameProviderCode,
				PlayerCode = playerCode
			},
			cancellationToken: HttpContext.RequestAborted);
}
