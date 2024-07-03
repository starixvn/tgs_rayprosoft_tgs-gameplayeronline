using NSubstitute;
using Tgs.GamePlayerOnline.Core.ApplicationServices;

namespace Tgs.GamePlayerOnline.Core.UnitTest.ApplicationServices;

public sealed class PlayerOnlineGetUpdatedBeforeServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakerRepository = Substitute.For<IPlayerOnlineRepository>();
		var sut = new PlayerOnlineGetUpdatedBeforeService(fakerRepository);
		var query = new PlayerOnlineGetUpdatedBeforeQuery(
			LastUpdateOn: new DateTime(2024, 07, 01, 13, 10, 00));

		await sut.QueryAsync(
				query: query,
				cancellationToken: default)
			.ToListAsync()
			.ConfigureAwait(true);

		_ = fakerRepository
			.Received(1)
			.GetUpdatedBeforeAsync(
				lastUpdateOn: Arg.Is(new DateTime(2024, 07, 01, 13, 10, 00)),
				cancellationToken: default);
	}
}
