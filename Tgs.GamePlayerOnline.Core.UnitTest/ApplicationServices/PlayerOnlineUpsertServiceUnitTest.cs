using System.Threading;
using Hangfire.MemoryStorage.Database;
using NSubstitute;
using Tgs.GamePlayerOnline.Core.ApplicationServices;
using Tgs.GamePlayerOnline.Core.Models;

namespace Tgs.GamePlayerOnline.Core.UnitTest.ApplicationServices;

public sealed class PlayerOnlineUpsertServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakerRepository = Substitute.For<IPlayerOnlineRepository>();
		var sut = new PlayerOnlineUpsertService(fakerRepository);
		var command = new PlayerOnlineUpsertCommand(
			GameProviderCode: "KY",
			PlayerCode: "4ucmqplxb029",
			Balance: 12m,
			FreeBalance: 7m,
			OutstandingBalance: 5m);

		_ = fakerRepository.GetAsync(
			gameProviderCode: Arg.Is(command.GameProviderCode),
			playerCode: Arg.Is(command.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult<PlayerOnline?>(null));

		await sut.ExecuteAsync(
				command: command,
				cancellationToken: default)
			.ConfigureAwait(true);

		_ = fakerRepository
			.Received(1)
			.UpsertAsync(
				gameProviderCode: Arg.Is("KY"),
				playerCode: Arg.Is("4ucmqplxb029"),
				balance: Arg.Is(12m),
				freeBalance: Arg.Is(7m),
				outstandingBalance: Arg.Is(5m),
				cancellationToken: default);
	}

	[Fact]
	public async Task The_data_are_exactly_the_same()
	{
		var fakerRepository = Substitute.For<IPlayerOnlineRepository>();
		var sut = new PlayerOnlineUpsertService(fakerRepository);
		var command = new PlayerOnlineUpsertCommand(
			GameProviderCode: "KY",
			PlayerCode: "4ucmqplxb029",
			Balance: 12m,
			FreeBalance: 7m,
			OutstandingBalance: 5m);

		_ = fakerRepository.GetAsync(
			gameProviderCode: Arg.Is(command.GameProviderCode),
			playerCode: Arg.Is(command.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(new PlayerOnline(
				GameProviderCode: "KY",
				PlayerCode: "4ucmqplxb029",
				Balance: 12.0m,
				FreeBalance: 7.0m,
				OutstandingBalance: 5.0m,
				UpdateOn: DateTime.UtcNow,
				ExpireOn: DateTime.UtcNow.AddHours(1)));

		await sut.ExecuteAsync(
				command: command,
				cancellationToken: default)
			.ConfigureAwait(true);

		_ = fakerRepository
			.Received(0)
			.UpsertAsync(
				gameProviderCode: Arg.Is("KY"),
				playerCode: Arg.Is("4ucmqplxb029"),
				balance: Arg.Is(12m),
				freeBalance: Arg.Is(7m),
				outstandingBalance: Arg.Is(5m),
				cancellationToken: default);
	}
}
