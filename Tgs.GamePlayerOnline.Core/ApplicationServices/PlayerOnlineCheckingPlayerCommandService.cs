using Tgs.GameFundTransfer.Client;
using Tgs.GamePlayerOnline.Protos;
using Tgs.GameProviders.Client;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GamePlayerOnline.Core.ApplicationServices;

internal class PlayerOnlineCheckingPlayerCommandService(
	IGameProvidersClient gameProvidersClient,
	IGameFundTransferClient gameFundTransferClient,
	IMessageSender messageSender) : ICommandService<PlayerOnlineCheckingPlayerCommand>
{
	public async ValueTask ExecuteAsync(PlayerOnlineCheckingPlayerCommand command, CancellationToken cancellationToken = default)
	{
		var fundBalanceReply = await gameFundTransferClient.FundBalanceAsync(
			gameProviderCode: command.GameProviderCode,
			playerCode: command.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (fundBalanceReply.Balance == 0)
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
			await messageSender.PublishAsync(
				subject: "game.gameplayeronline.upsert.request",
				data: new PlayerOnlineUpsertRequest
				{
					GameProviderCode = command.GameProviderCode,
					PlayerCode = command.PlayerCode,
					Balance = fundBalanceReply.Balance,
					FreeBalance = fundBalanceReply.FreeBalance,
					OutstandingBalance = fundBalanceReply.OutstandingBalance
				},
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
			return;
		}

		/*
		 * 遊戲商餘額跟記錄餘額相等且玩家不在線
		 * 1.遊戲商餘額轉回主錢包
		 * 2.刪除紀錄資料
		 */
	}
}
