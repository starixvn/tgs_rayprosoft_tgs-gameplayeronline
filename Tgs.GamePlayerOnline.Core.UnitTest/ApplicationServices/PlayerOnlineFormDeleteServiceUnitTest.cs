using NSubstitute;
using Tgs.GamePlayerOnline.Core.ApplicationServices;

namespace Tgs.GamePlayerOnline.Core.UnitTest.ApplicationServices;

public class PlayerOnlineFormDeleteServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakerRepository = Substitute.For<IPlayerOnlineRepository>();
		var sut = new PlayerOnlineFormDeleteService(fakerRepository);
		var command = new PlayerOnlineFormDeleteCommand(
			GameProviderCode: "KY",
			PlayerCode: "4ucmqplxb029");

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
}
