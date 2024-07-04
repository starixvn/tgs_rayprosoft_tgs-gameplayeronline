using Microsoft.AspNetCore.Mvc;
using Tgs.GamePlayerOnline.Core.ApplicationServices;
using Tgs.GamePlayerOnline.Protos;
using Tgs.GamePlayerOnline.ViewModels;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GamePlayerOnline.Controllers;

[Route("[controller]")]
[ApiController]
public class PlayerOnlineController : ControllerBase
{
	/// <summary>
	/// Gets the asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="gameProviderCode">The game provider code. Example: <example>KY</example></param>
	/// <param name="playerCode">The player code. Example: <example>4ucmqplxb029</example></param>
	/// <returns></returns>
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

	/// <summary>
	/// Gets the updated before asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="lastUpdateOn">The last update on. Example: <example>2024-07-01 17:00:00</example></param>
	/// <returns></returns>
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
	/// <param name="gameProviderCode">The game provider code. Example: <example>KY</example></param>
	/// <param name="playerCode">The player code. Example: <example>4ucmqplxb029</example></param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPut("{gameProviderCode}/{playerCode}")]
	public ValueTask UpsertAsync(
		[FromServices] IMessageSender messageSender,
		string gameProviderCode,
		string playerCode,
		PlayerOnlineUpsertViewModel source)
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

	/// <summary>
	/// Deletes the asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="gameProviderCode">The game provider code. Example: <example>KY</example></param>
	/// <param name="playerCode">The player code. Example: <example>4ucmqplxb029</example></param>
	/// <returns></returns>
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

	[HttpPost("Clean")]
	public ValueTask CleanAsync(
		[FromServices] IMessageSender messageSender)
		=> messageSender.PublishAsync(
			subject: "game.gameplayeronline.clean.request",
			data: new PlayerOnlineCleanRequest(),
			cancellationToken: HttpContext.RequestAborted);
}
