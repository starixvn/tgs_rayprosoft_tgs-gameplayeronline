using Microsoft.Extensions.Logging;
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
		var playerBalanceInfo = await backofficePluggableClient.GetBalanceInfoAsync(
			playerCode: command.PlayerCode,
			gameProviderCode: command.GameProviderCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (playerBalanceInfo.Balance == 0 || playerBalanceInfo.FreeBalance <= 0)
		{
			return;
		}

		var online = command.Balance != playerBalanceInfo.Balance
			|| command.FreeBalance != playerBalanceInfo.FreeBalance
			|| command.OutstandingBalance != playerBalanceInfo.OutstandingBalance;

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
			logger.LogInformation("{logInformation}",
				new
				{
					LongOn = DateTime.Now.ToString("o"),
					command.PlayerCode,
					command.GameProviderCode,
					playerBalanceInfo.Balance,
					playerBalanceInfo.FreeBalance,
					playerBalanceInfo.OutstandingBalance,
					Message = "玩家在線"
				});
			return;
		}

		if (playerBalanceInfo.FreeBalance >= 1)
		{
			return;
		}

		/*
		 * 遊戲商餘額跟記錄餘額相等且玩家不在線
		 * 1.登出玩家
		 * 2.遊戲商餘額轉回主錢包
		 * 3.刪除紀錄資料
		 */

		logger.LogInformation("{logInformation}",
			new
			{
				LongOn = DateTime.Now.ToString("o"),
				command.PlayerCode,
				command.GameProviderCode,
				playerBalanceInfo.Balance,
				playerBalanceInfo.FreeBalance,
				playerBalanceInfo.OutstandingBalance,
				Message = "玩家遊戲錢包轉入主錢包-執行"
			});

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
					playerBalanceInfo.Balance,
					playerBalanceInfo.FreeBalance,
					playerBalanceInfo.OutstandingBalance,
					Message = "玩家登出-異常"
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
					playerBalanceInfo.Balance,
					playerBalanceInfo.FreeBalance,
					playerBalanceInfo.OutstandingBalance,
					Message = "玩家遊戲錢包轉入主錢包-成功"
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
					playerBalanceInfo.Balance,
					playerBalanceInfo.FreeBalance,
					playerBalanceInfo.OutstandingBalance,
					Message = "玩家遊戲錢包轉入主錢包-異常"
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
