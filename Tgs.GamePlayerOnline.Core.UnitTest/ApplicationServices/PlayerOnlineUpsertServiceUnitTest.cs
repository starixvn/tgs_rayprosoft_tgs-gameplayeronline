using NSubstitute;
using Tgs.GamePlayerOnline.Core.ApplicationServices;

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
}
