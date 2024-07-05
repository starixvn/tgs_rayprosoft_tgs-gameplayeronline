using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GamePlayerOnline.Core.ApplicationServices;
using Tgs.GamePlayerOnline.Protos;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Pluggable.Client;
using Valhalla.MessageQueue;

namespace Tgs.GamePlayerOnline.Core.UnitTest.ApplicationServices;

public class PlayerOnlineCheckingPlayerCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeLog = NullLogger<PlayerOnlineCheckingPlayerCommandService>.Instance;
		var fakerGameProvidersClient = Substitute.For<IGameProvidersClient>();
		var fakerBackofficePluggableClient = Substitute.For<IGameProvidersBackofficePluggableClient>();
		var fakerMessageSender = Substitute.For<IMessageSender>();

		var command = new PlayerOnlineCheckingPlayerCommand(
			GameProviderCode: "KY",
			PlayerCode: "4ucmqplxb029",
			Balance: 0.123m,
			FreeBalance: 0.123m,
			OutstandingBalance: 0.0m);

		var playerBalanceInfo = new GameProviders.Pluggable.ViewModels.PlayerBalanceInfoViewModel(
			Balance: 0.123m,
			FreeBalance: 0.123m,
			OutstandingBalance: 0.0m
		);
		_ = fakerBackofficePluggableClient.GetBalanceInfoAsync(
			playerCode: Arg.Is(command.PlayerCode),
			gameProviderCode: Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerBalanceInfo);

		var playerOnlineResponse = new PlayerOnlineResponse(
			GameProviderSupport: true,
			Online: false);
		_ = fakerGameProvidersClient.PlayerOnlineAsync(
			gameProviderCode: Arg.Is(command.GameProviderCode),
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerOnlineResponse);

		var sut = new PlayerOnlineCheckingPlayerCommandService(
			fakeLog,
			fakerGameProvidersClient,
			fakerBackofficePluggableClient,
			fakerMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakerGameProvidersClient
			.Received(1)
			.PlayerLogoutAsync(
			gameProviderCode: Arg.Is(command.GameProviderCode),
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakerBackofficePluggableClient
			.Received(1)
			.FundInAsync(
			playerCode: Arg.Is(command.PlayerCode),
			gameProviderCode: Arg.Is(command.GameProviderCode),

			cancellationToken: Arg.Any<CancellationToken>());

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
	}

	[Fact]
	public async Task Balance_Is_Chang()
	{
		var fakeLog = NullLogger<PlayerOnlineCheckingPlayerCommandService>.Instance;
		var fakerGameProvidersClient = Substitute.For<IGameProvidersClient>();
		var fakerBackofficePluggableClient = Substitute.For<IGameProvidersBackofficePluggableClient>();
		var fakerMessageSender = Substitute.For<IMessageSender>();

		var command = new PlayerOnlineCheckingPlayerCommand(
			GameProviderCode: "KY",
			PlayerCode: "4ucmqplxb029",
			Balance: 50.0m,
			FreeBalance: 0,
			OutstandingBalance: 50);

		var playerBalanceInfo = new GameProviders.Pluggable.ViewModels.PlayerBalanceInfoViewModel(
			Balance: 100,
			FreeBalance: 100,
			OutstandingBalance: 0
		);
		_ = fakerBackofficePluggableClient.GetBalanceInfoAsync(
			playerCode: Arg.Is(command.PlayerCode),
			gameProviderCode: Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerBalanceInfo);

		var sut = new PlayerOnlineCheckingPlayerCommandService(
			fakeLog,
			fakerGameProvidersClient,
			fakerBackofficePluggableClient,
			fakerMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakerGameProvidersClient
			.Received(0)
			.PlayerLogoutAsync(
			gameProviderCode: Arg.Is(command.GameProviderCode),
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakerBackofficePluggableClient
			.Received(0)
			.FundInAsync(
			playerCode: Arg.Is(command.PlayerCode),
			gameProviderCode: Arg.Is(command.GameProviderCode),

			cancellationToken: Arg.Any<CancellationToken>());

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
	}

	[Fact]
	public async Task Online_Is_True()
	{
		var fakeLog = NullLogger<PlayerOnlineCheckingPlayerCommandService>.Instance;
		var fakerGameProvidersClient = Substitute.For<IGameProvidersClient>();
		var fakerBackofficePluggableClient = Substitute.For<IGameProvidersBackofficePluggableClient>();
		var fakerMessageSender = Substitute.For<IMessageSender>();

		var command = new PlayerOnlineCheckingPlayerCommand(
			GameProviderCode: "KY",
			PlayerCode: "4ucmqplxb029",
			Balance: 50,
			FreeBalance: 0,
			OutstandingBalance: 50);

		var playerBalanceInfo = new GameProviders.Pluggable.ViewModels.PlayerBalanceInfoViewModel(
			Balance: 50.0m,
			FreeBalance: 0.0m,
			OutstandingBalance: 50.0m
		);
		_ = fakerBackofficePluggableClient.GetBalanceInfoAsync(
			playerCode: Arg.Is(command.PlayerCode),
			gameProviderCode: Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerBalanceInfo);

		var playerOnlineResponse = new PlayerOnlineResponse(
			GameProviderSupport: true,
			Online: true);
		_ = fakerGameProvidersClient.PlayerOnlineAsync(
			gameProviderCode: Arg.Is(command.GameProviderCode),
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerOnlineResponse);

		var sut = new PlayerOnlineCheckingPlayerCommandService(
			fakeLog,
			fakerGameProvidersClient,
			fakerBackofficePluggableClient,
			fakerMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakerGameProvidersClient
			.Received(0)
			.PlayerLogoutAsync(
			gameProviderCode: Arg.Is(command.GameProviderCode),
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakerBackofficePluggableClient
			.Received(0)
			.FundInAsync(
			playerCode: Arg.Is(command.PlayerCode),
			gameProviderCode: Arg.Is(command.GameProviderCode),

			cancellationToken: Arg.Any<CancellationToken>());

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
	}

	[Fact]
	public async Task FreeBalance_is_0()
	{
		var fakeLog = NullLogger<PlayerOnlineCheckingPlayerCommandService>.Instance;
		var fakerGameProvidersClient = Substitute.For<IGameProvidersClient>();
		var fakerBackofficePluggableClient = Substitute.For<IGameProvidersBackofficePluggableClient>();
		var fakerMessageSender = Substitute.For<IMessageSender>();

		var command = new PlayerOnlineCheckingPlayerCommand(
			GameProviderCode: "KY",
			PlayerCode: "4ucmqplxb029",
			Balance: 50.0m,
			FreeBalance: 0.0m,
			OutstandingBalance: 50.0m);

		var playerBalanceInfo = new GameProviders.Pluggable.ViewModels.PlayerBalanceInfoViewModel(
			Balance: 50.0m,
			FreeBalance: 0.0m,
			OutstandingBalance: 50.0m
		);
		_ = fakerBackofficePluggableClient.GetBalanceInfoAsync(
			playerCode: Arg.Is(command.PlayerCode),
			gameProviderCode: Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerBalanceInfo);

		var sut = new PlayerOnlineCheckingPlayerCommandService(
			fakeLog,
			fakerGameProvidersClient,
			fakerBackofficePluggableClient,
			fakerMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakerGameProvidersClient
			.Received(0)
			.PlayerOnlineAsync(
			gameProviderCode: Arg.Is(command.GameProviderCode),
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakerGameProvidersClient
			.Received(0)
			.PlayerLogoutAsync(
			gameProviderCode: Arg.Is(command.GameProviderCode),
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakerBackofficePluggableClient
			.Received(0)
			.FundInAsync(
			playerCode: Arg.Is(command.PlayerCode),
			gameProviderCode: Arg.Is(command.GameProviderCode),

			cancellationToken: Arg.Any<CancellationToken>());

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
	}

	[Fact]
	public async Task Balance_Is_0()
	{
		var fakeLog = NullLogger<PlayerOnlineCheckingPlayerCommandService>.Instance;
		var fakerGameProvidersClient = Substitute.For<IGameProvidersClient>();
		var fakerBackofficePluggableClient = Substitute.For<IGameProvidersBackofficePluggableClient>();
		var fakerMessageSender = Substitute.For<IMessageSender>();

		var command = new PlayerOnlineCheckingPlayerCommand(
			GameProviderCode: "KY",
			PlayerCode: "4ucmqplxb029",
			Balance: 50,
			FreeBalance: 0,
			OutstandingBalance: 50);

		var playerBalanceInfo = new GameProviders.Pluggable.ViewModels.PlayerBalanceInfoViewModel(
			Balance: 0.0m,
			FreeBalance: 0.0m,
			OutstandingBalance: 0.0m
		);
		_ = fakerBackofficePluggableClient.GetBalanceInfoAsync(
			playerCode: Arg.Is(command.PlayerCode),
			gameProviderCode: Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerBalanceInfo);

		var sut = new PlayerOnlineCheckingPlayerCommandService(
			fakeLog,
			fakerGameProvidersClient,
			fakerBackofficePluggableClient,
			fakerMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakerGameProvidersClient
			.Received(0)
			.PlayerLogoutAsync(
			gameProviderCode: Arg.Is(command.GameProviderCode),
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakerBackofficePluggableClient
			.Received(0)
			.FundInAsync(
			playerCode: Arg.Is(command.PlayerCode),
			gameProviderCode: Arg.Is(command.GameProviderCode),

			cancellationToken: Arg.Any<CancellationToken>());

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
	}

	[Fact]
	public async Task Balance_is_greater_than_or_equal_to_1()
	{
		var fakeLog = NullLogger<PlayerOnlineCheckingPlayerCommandService>.Instance;
		var fakerGameProvidersClient = Substitute.For<IGameProvidersClient>();
		var fakerBackofficePluggableClient = Substitute.For<IGameProvidersBackofficePluggableClient>();
		var fakerMessageSender = Substitute.For<IMessageSender>();

		var command = new PlayerOnlineCheckingPlayerCommand(
			GameProviderCode: "KY",
			PlayerCode: "4ucmqplxb029",
			Balance: 1.0m,
			FreeBalance: 1.0m,
			OutstandingBalance: 0.0m);

		var playerBalanceInfo = new GameProviders.Pluggable.ViewModels.PlayerBalanceInfoViewModel(
			Balance: 1.0m,
			FreeBalance: 1.0m,
			OutstandingBalance: 0.0m
		);
		_ = fakerBackofficePluggableClient.GetBalanceInfoAsync(
			playerCode: Arg.Is(command.PlayerCode),
			gameProviderCode: Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerBalanceInfo);

		var playerOnlineResponse = new PlayerOnlineResponse(
			GameProviderSupport: true,
			Online: false);
		_ = fakerGameProvidersClient.PlayerOnlineAsync(
			gameProviderCode: Arg.Is(command.GameProviderCode),
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerOnlineResponse);

		var sut = new PlayerOnlineCheckingPlayerCommandService(
			fakeLog,
			fakerGameProvidersClient,
			fakerBackofficePluggableClient,
			fakerMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakerGameProvidersClient
			.Received(0)
			.PlayerLogoutAsync(
			gameProviderCode: Arg.Is(command.GameProviderCode),
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakerBackofficePluggableClient
			.Received(0)
			.FundInAsync(
			playerCode: Arg.Is(command.PlayerCode),
			gameProviderCode: Arg.Is(command.GameProviderCode),

			cancellationToken: Arg.Any<CancellationToken>());

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
	}
}
