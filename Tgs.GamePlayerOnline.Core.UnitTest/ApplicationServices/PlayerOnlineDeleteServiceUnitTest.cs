using NSubstitute;
using Tgs.GamePlayerOnline.Core.ApplicationServices;

namespace Tgs.GamePlayerOnline.Core.UnitTest.ApplicationServices;

public sealed class PlayerOnlineDeleteServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakerRepository = Substitute.For<IPlayerOnlineRepository>();
		var sut = new PlayerOnlineDeleteService(fakerRepository);
		var command = new PlayerOnlineDeleteCommand(
			GameProviderCode: "KY",
			PlayerCode: "4ucmqplxb029");

		_ = fakerRepository.GetAsync(
			gameProviderCode: Arg.Is(command.GameProviderCode),
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(new Models.PlayerOnline(
				GameProviderCode: "KY",
				PlayerCode: "4ucmqplxb029",
				Balance: 0.0m,
				FreeBalance: 0.0m,
				OutstandingBalance: 0.0m,
				UpdateOn: DateTime.UtcNow,
				ExpireOn: DateTime.UtcNow));

		await sut.ExecuteAsync(
				command: command,
				cancellationToken: default)
			.ConfigureAwait(true);

		_ = fakerRepository
			.Received(1)
			.DeleteAsync(
				gameProviderCode: Arg.Is("KY"),
				playerCode: Arg.Is("4ucmqplxb029"),
				cancellationToken: default);
	}

	[Fact]
	public async Task Data_Is_Null()
	{
		var fakerRepository = Substitute.For<IPlayerOnlineRepository>();
		var sut = new PlayerOnlineDeleteService(fakerRepository);
		var command = new PlayerOnlineDeleteCommand(
			GameProviderCode: "KY",
			PlayerCode: "4ucmqplxb029");

		_ = fakerRepository.GetAsync(
			gameProviderCode: Arg.Is(command.GameProviderCode),
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((Models.PlayerOnline?)null);

		await sut.ExecuteAsync(
				command: command,
				cancellationToken: default)
			.ConfigureAwait(true);

		_ = fakerRepository
			.Received(0)
			.DeleteAsync(
				gameProviderCode: Arg.Is("KY"),
				playerCode: Arg.Is("4ucmqplxb029"),
				cancellationToken: default);
	}
}
