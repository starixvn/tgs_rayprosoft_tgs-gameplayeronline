using System.Threading;
using NSubstitute;
using Tgs.GameFundTransfer.Client;
using Tgs.GameFundTransfer.Protos;
using Tgs.GamePlayerOnline.Core.ApplicationServices;
using Tgs.GamePlayerOnline.Protos;
using Tgs.GameProviders.Client;
using Valhalla.MessageQueue;

namespace Tgs.GamePlayerOnline.Core.UnitTest.ApplicationServices;

public class PlayerOnlineCheckingPlayerCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakerGameProvidersClient = Substitute.For<IGameProvidersClient>();
		var fakerGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
		var fakerMessageSender = Substitute.For<IMessageSender>();

		var command = new PlayerOnlineCheckingPlayerCommand(
			GameProviderCode: "KY",
			PlayerCode: "4ucmqplxb029",
			Balance: 50,
			FreeBalance: 0,
			OutstandingBalance: 50);

		var fundBalanceReply = new FundBalanceReply
		{
			Balance = 50,
			FreeBalance = 0,
			OutstandingBalance = 50
		};
		_ = fakerGameFundTransferClient.FundBalanceAsync(
			gameProviderCode: Arg.Is(command.GameProviderCode),
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(fundBalanceReply);

		var playerOnlineResponse = new PlayerOnlineResponse(
			GameProviderSupport: true,
			Online: false);
		_ = fakerGameProvidersClient.PlayerOnlineAsync(
			gameProviderCode: Arg.Is(command.GameProviderCode),
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerOnlineResponse);

		var sut = new PlayerOnlineCheckingPlayerCommandService(
			fakerGameProvidersClient,
			fakerGameFundTransferClient,
			fakerMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakerMessageSender
			.Received(0)
			.PublishAsync(
			subject: Arg.Is("game.gameplayeronline.delete.request"),
			data: Arg.Is(new PlayerOnlineDeleteRequest
			{
				GameProviderCode = command.GameProviderCode,
				PlayerCode = command.PlayerCode
			}),
			Arg.Any<CancellationToken>());

		_ = fakerMessageSender
			.Received(0)
			.PublishAsync(
			subject: Arg.Is("game.gameplayeronline.upsert.request"),
			data: Arg.Is(new PlayerOnlineUpsertRequest
			{
				GameProviderCode = command.GameProviderCode,
				PlayerCode = command.PlayerCode,
				Balance = fundBalanceReply.Balance,
				FreeBalance = fundBalanceReply.FreeBalance,
				OutstandingBalance = fundBalanceReply.OutstandingBalance
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Balance_Is_Chang()
	{
		var fakerGameProvidersClient = Substitute.For<IGameProvidersClient>();
		var fakerGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
		var fakerMessageSender = Substitute.For<IMessageSender>();

		var command = new PlayerOnlineCheckingPlayerCommand(
			GameProviderCode: "KY",
			PlayerCode: "4ucmqplxb029",
			Balance: 50,
			FreeBalance: 0,
			OutstandingBalance: 50);

		var fundBalanceReply = new FundBalanceReply
		{
			Balance = 100,
			FreeBalance = 100,
			OutstandingBalance = 0
		};
		_ = fakerGameFundTransferClient.FundBalanceAsync(
			gameProviderCode: Arg.Is(command.GameProviderCode),
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(fundBalanceReply);

		var sut = new PlayerOnlineCheckingPlayerCommandService(
			fakerGameProvidersClient,
			fakerGameFundTransferClient,
			fakerMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakerMessageSender
			.Received(0)
			.PublishAsync(
			subject: Arg.Is("game.gameplayeronline.delete.request"),
			data: Arg.Is(new PlayerOnlineDeleteRequest
			{
				GameProviderCode = command.GameProviderCode,
				PlayerCode = command.PlayerCode
			}),
			Arg.Any<CancellationToken>());

		_ = fakerMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is("game.gameplayeronline.upsert.request"),
			data: Arg.Is(new PlayerOnlineUpsertRequest
			{
				GameProviderCode = command.GameProviderCode,
				PlayerCode = command.PlayerCode,
				Balance = fundBalanceReply.Balance,
				FreeBalance = fundBalanceReply.FreeBalance,
				OutstandingBalance = fundBalanceReply.OutstandingBalance
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Online_Is_True()
	{
		var fakerGameProvidersClient = Substitute.For<IGameProvidersClient>();
		var fakerGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
		var fakerMessageSender = Substitute.For<IMessageSender>();

		var command = new PlayerOnlineCheckingPlayerCommand(
			GameProviderCode: "KY",
			PlayerCode: "4ucmqplxb029",
			Balance: 50,
			FreeBalance: 0,
			OutstandingBalance: 50);

		var fundBalanceReply = new FundBalanceReply
		{
			Balance = 50,
			FreeBalance = 0,
			OutstandingBalance = 50
		};
		_ = fakerGameFundTransferClient.FundBalanceAsync(
			gameProviderCode: Arg.Is(command.GameProviderCode),
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(fundBalanceReply);

		var playerOnlineResponse = new PlayerOnlineResponse(
			GameProviderSupport: true,
			Online: true);
		_ = fakerGameProvidersClient.PlayerOnlineAsync(
			gameProviderCode: Arg.Is(command.GameProviderCode),
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerOnlineResponse);

		var sut = new PlayerOnlineCheckingPlayerCommandService(
			fakerGameProvidersClient,
			fakerGameFundTransferClient,
			fakerMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakerMessageSender
			.Received(0)
			.PublishAsync(
			subject: Arg.Is("game.gameplayeronline.delete.request"),
			data: Arg.Is(new PlayerOnlineDeleteRequest
			{
				GameProviderCode = command.GameProviderCode,
				PlayerCode = command.PlayerCode
			}),
			Arg.Any<CancellationToken>());

		_ = fakerMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is("game.gameplayeronline.upsert.request"),
			data: Arg.Is(new PlayerOnlineUpsertRequest
			{
				GameProviderCode = command.GameProviderCode,
				PlayerCode = command.PlayerCode,
				Balance = fundBalanceReply.Balance,
				FreeBalance = fundBalanceReply.FreeBalance,
				OutstandingBalance = fundBalanceReply.OutstandingBalance
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Balance_Is_0()
	{
		var fakerGameProvidersClient = Substitute.For<IGameProvidersClient>();
		var fakerGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
		var fakerMessageSender = Substitute.For<IMessageSender>();

		var command = new PlayerOnlineCheckingPlayerCommand(
			GameProviderCode: "KY",
			PlayerCode: "4ucmqplxb029",
			Balance: 50,
			FreeBalance: 0,
			OutstandingBalance: 50);

		var fundBalanceReply = new FundBalanceReply
		{
			Balance = 0,
			FreeBalance = 0,
			OutstandingBalance = 0
		};
		_ = fakerGameFundTransferClient.FundBalanceAsync(
			gameProviderCode: Arg.Is(command.GameProviderCode),
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(fundBalanceReply);

		var sut = new PlayerOnlineCheckingPlayerCommandService(
			fakerGameProvidersClient,
			fakerGameFundTransferClient,
			fakerMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakerMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is("game.gameplayeronline.delete.request"),
			data: Arg.Is(new PlayerOnlineDeleteRequest
			{
				GameProviderCode = command.GameProviderCode,
				PlayerCode = command.PlayerCode
			}),
			Arg.Any<CancellationToken>());

		_ = fakerMessageSender
			.Received(0)
			.PublishAsync(
			subject: Arg.Is("game.gameplayeronline.upsert.request"),
			data: Arg.Is(new PlayerOnlineUpsertRequest
			{
				GameProviderCode = command.GameProviderCode,
				PlayerCode = command.PlayerCode,
				Balance = fundBalanceReply.Balance,
				FreeBalance = fundBalanceReply.FreeBalance,
				OutstandingBalance = fundBalanceReply.OutstandingBalance
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
