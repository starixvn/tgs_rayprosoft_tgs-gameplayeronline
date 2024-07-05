using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GamePlayerOnline.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GamePlayerOnline.Core.UnitTest.ApplicationServices;

public sealed class PlayerOnlineCleanCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var faceLogger = NullLogger<PlayerOnlineCleanCommandService>.Instance;
		var fakeFundInWhitelistOptions = Options.Create(new FundInWhitelistOptions
		{
			ReleaseNamespace = "dev",
			ReleaseNamespaces = ["dev", "staging"]
		});
		var fakerQueryService = Substitute.For<IQueryService<PlayerOnlineGetUpdatedBeforeQuery, PlayerOnlineGetResult>>();
		var fakerCheckingPlayerService = Substitute.For<ICommandService<PlayerOnlineCheckingPlayerCommand>>();

		var sut = new PlayerOnlineCleanCommandService(
			faceLogger,
			fakeFundInWhitelistOptions,
			fakerQueryService,
			fakerCheckingPlayerService);

		var command = new PlayerOnlineCleanCommand(
			Deadline: new DateTime(2024, 07, 01, 17, 00, 00));
		var playerOnline = new PlayerOnlineGetResult(
			GameProviderCode: "KY",
			PlayerCode: "4ucmqplxb029",
			Balance: 7,
			FreeBalance: 4,
			OutstandingBalance: 3);
		_ = fakerQueryService
			.QueryAsync(
				query: Arg.Is<PlayerOnlineGetUpdatedBeforeQuery>(cmd =>
					cmd.LastUpdateOn == new DateTime(2024, 07, 01, 17, 00, 00)),
				cancellationToken: default)
			.Returns(new[] { playerOnline }.ToAsyncEnumerable());

		await sut.ExecuteAsync(
				command: command,
				cancellationToken: default)
			.ConfigureAwait(true);

		_ = fakerCheckingPlayerService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new PlayerOnlineCheckingPlayerCommand(
				GameProviderCode: playerOnline.GameProviderCode,
				PlayerCode: playerOnline.PlayerCode,
				Balance: playerOnline.Balance,
				FreeBalance: playerOnline.FreeBalance,
				OutstandingBalance: playerOnline.OutstandingBalance)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task FundsIn_the_whitelist_are_not_in_the_list()
	{
		var faceLogger = NullLogger<PlayerOnlineCleanCommandService>.Instance;
		var fakeFundInWhitelistOptions = Options.Create(new FundInWhitelistOptions
		{
			ReleaseNamespace = "prod",
			ReleaseNamespaces = ["dev", "staging"]
		});
		var fakerQueryService = Substitute.For<IQueryService<PlayerOnlineGetUpdatedBeforeQuery, PlayerOnlineGetResult>>();
		var fakerCheckingPlayerService = Substitute.For<ICommandService<PlayerOnlineCheckingPlayerCommand>>();

		var sut = new PlayerOnlineCleanCommandService(
			faceLogger,
			fakeFundInWhitelistOptions,
			fakerQueryService,
			fakerCheckingPlayerService);

		var command = new PlayerOnlineCleanCommand(
			Deadline: new DateTime(2024, 07, 01, 17, 00, 00));

		await sut.ExecuteAsync(
				command: command,
				cancellationToken: default)

			.ConfigureAwait(true);

		_ = fakerQueryService
			.Received(0)
			.QueryAsync(
			query: Arg.Any<PlayerOnlineGetUpdatedBeforeQuery>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakerCheckingPlayerService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<PlayerOnlineCheckingPlayerCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
