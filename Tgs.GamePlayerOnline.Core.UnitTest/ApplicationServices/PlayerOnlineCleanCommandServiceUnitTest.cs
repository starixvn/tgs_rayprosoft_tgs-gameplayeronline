using NSubstitute;
using Tgs.GamePlayerOnline.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GamePlayerOnline.Core.UnitTest.ApplicationServices;

public sealed class PlayerOnlineCleanCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakerQueryService = Substitute.For<IQueryService<PlayerOnlineGetUpdatedBeforeQuery, PlayerOnlineGetResult>>();
		var fakerDeleteService = Substitute.For<ICommandService<PlayerOnlineDeleteCommand>>();
		var sut = new PlayerOnlineCleanCommandService(fakerQueryService, fakerDeleteService);
		var command = new PlayerOnlineCleanCommand(
			Deadline: new DateTime(2024, 07, 01, 17, 00, 00));
		fakerQueryService
			.QueryAsync(
				query: Arg.Is<PlayerOnlineGetUpdatedBeforeQuery>(cmd =>
					cmd.LastUpdateOn == new DateTime(2024, 07, 01, 17, 00, 00)),
				cancellationToken: default)
			.Returns(new[] { new PlayerOnlineGetResult(
				GameProviderCode: "KY",
				BrandCode: "KY",
				PlayerCode: "4ucmqplxb029",
				Username: "4ucmqplxb029",
				Balance: 7,
				FreeBalance: 4,
				OutstandingBalance: 3) }.ToAsyncEnumerable());

		await sut.ExecuteAsync(
				command: command,
				cancellationToken: default)
			.ConfigureAwait(true);

		_ = fakerDeleteService
			.Received(1)
			.ExecuteAsync(
				command: Arg.Is<PlayerOnlineDeleteCommand>(cmd =>
					cmd.GameProviderCode == "KY" &&
					cmd.PlayerCode == "4ucmqplxb029"),
				cancellationToken: default);
	}
}
