using NSubstitute;
using Tgs.GamePlayerOnline.Core.ApplicationServices;

namespace Tgs.GamePlayerOnline.Core.UnitTest.ApplicationServices;

public class PlayerOnlineFormGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakerRepository = Substitute.For<IPlayerOnlineRepository>();
		var sut = new PlayerOnlineFormGetService(fakerRepository);
		var query = new PlayerOnlineFormGetQuery(
			GameProviderCode: "KY",
			PlayerCode: "4ucmqplxb029");

		await sut.GetAsync(
				query: query,
				cancellationToken: default)
			.ConfigureAwait(true);

		_ = fakerRepository
			.Received(1)
			.GetAsync(
				gameProviderCode: Arg.Is("KY"),
				playerCode: Arg.Is("4ucmqplxb029"),
				cancellationToken: default);
	}
}
