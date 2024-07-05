using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GamePlayerOnline.Protos;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

internal class PlayerOnlineCheckingPlayerCommandService(
	ILogger<PlayerOnlineCheckingPlayerCommandService> logger,
	IGameProvidersClient gameProvidersClient,
	IGameProvidersBackofficePluggableClient backofficePluggableClient,
	IMessageSender messageSender) : ICommandService<PlayerOnlineCheckingPlayerCommand>
{
	public async ValueTask ExecuteAsync(PlayerOnlineCheckingPlayerCommand command, CancellationToken cancellationToken = default)
	{
		var fundBalanceReply = await backofficePluggableClient.GetBalanceInfoAsync(
			playerCode: command.PlayerCode,
			gameProviderCode: command.GameProviderCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (fundBalanceReply.Balance == 0 || fundBalanceReply.FreeBalance <= 0)
		{
			return;
		}

		var online = command.Balance != fundBalanceReply.Balance
			|| command.FreeBalance != fundBalanceReply.FreeBalance
			|| command.OutstandingBalance != fundBalanceReply.OutstandingBalance;

		if (!online)
		{
			var playerOnlineResponse = await gameProvidersClient.PlayerOnlineAsync(
				gameProviderCode: command.GameProviderCode,
				playerCode: command.PlayerCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			online = playerOnlineResponse.GameProviderSupport && playerOnlineResponse.Online;
		}

		if (online)
		{
			return;
		}

		if (fundBalanceReply.FreeBalance is >= 1)
		{
			return;
		}

		/*
		 * 遊戲商餘額跟記錄餘額相等且玩家不在線
		 * 1.登出玩家
		 * 2.遊戲商餘額轉回主錢包
		 * 3.刪除紀錄資料
		 */
		try
		{
			var playerLogoutResponse = await gameProvidersClient.PlayerLogoutAsync(
				command.GameProviderCode,
				command.PlayerCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			if (playerLogoutResponse.GameProviderSupport)
			{
				await Task.Delay(
					TimeSpan.FromSeconds(1),
					cancellationToken)
					.ConfigureAwait(false);
			}
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "{logError}",
				new
				{
					LongOn = DateTime.Now.ToString("o"),
					command.PlayerCode,
					command.GameProviderCode,
					Message = "玩家登出功能發生異常。"
				});
		}

		try
		{
			await backofficePluggableClient.FundInAsync(
				playerCode: command.PlayerCode,
				gameProviderCode: command.GameProviderCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			logger.LogInformation("{logInformation}",
				new
				{
					LongOn = DateTime.Now.ToString("o"),
					command.PlayerCode,
					command.GameProviderCode,
					Message = "玩家遊戲錢包轉匯主錢包功能成功。"
				});
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "{logError}",
				new
				{
					LongOn = DateTime.Now.ToString("o"),
					command.PlayerCode,
					command.GameProviderCode,
					Message = "玩家遊戲錢包轉匯主錢包功能發生異常。"
				});
		}
		finally
		{
			await messageSender.PublishAsync(
				subject: "game.gameplayeronline.delete.request",
				data: new PlayerOnlineDeleteRequest
				{
					GameProviderCode = command.GameProviderCode,
					PlayerCode = command.PlayerCode
				},
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
	}
}
