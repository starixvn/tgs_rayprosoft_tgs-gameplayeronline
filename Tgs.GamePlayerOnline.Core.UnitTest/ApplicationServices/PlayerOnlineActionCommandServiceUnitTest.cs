using NSubstitute;
using Tgs.GamePlayerOnline.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GamePlayerOnline.Core.UnitTest.ApplicationServices;

public sealed class PlayerOnlineActionCommandServiceUnitTest
{
	[Fact]
	public async Task IsUpsert()
	{
		var fakerUpsertService = Substitute.For<ICommandService<PlayerOnlineUpsertCommand>>();
		var fakerDeleteService = Substitute.For<ICommandService<PlayerOnlineDeleteCommand>>();
		var sut = new PlayerOnlineActionCommandService(fakerUpsertService, fakerDeleteService);
		var command = new PlayerOnlineActionCommand(
			GameProviderCode: "KY",
			PlayerCode: "4ucmqplxb029",
			IsUpsert: true,
			Balance: 7,
			FreeBalance: 3,
			OutstandingBalance: 4);

		await sut.ExecuteAsync(
				command: command,
				cancellationToken: default)
			.ConfigureAwait(true);

		_ = fakerUpsertService
			.Received(1)
			.ExecuteAsync(
				command: Arg.Is<PlayerOnlineUpsertCommand>(cmd =>
					cmd.GameProviderCode == "KY" &&
					cmd.PlayerCode == "4ucmqplxb029" &&
					cmd.Balance == 7 &&
					cmd.FreeBalance == 3 &&
					cmd.OutstandingBalance == 4),
				cancellationToken: default);
	}

	[Fact]
	public async Task IsNotUpsert()
	{
		var fakerUpsertService = Substitute.For<ICommandService<PlayerOnlineUpsertCommand>>();
		var fakerDeleteService = Substitute.For<ICommandService<PlayerOnlineDeleteCommand>>();
		var sut = new PlayerOnlineActionCommandService(fakerUpsertService, fakerDeleteService);
		var command = new PlayerOnlineActionCommand(
			GameProviderCode: "KY",
			PlayerCode: "4ucmqplxb029",
			IsUpsert: false,
			Balance: 0,
			FreeBalance: 0,
			OutstandingBalance: 0);

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

	[Fact]
	public async Task Balance_Is_0()
	{
		var fakerUpsertService = Substitute.For<ICommandService<PlayerOnlineUpsertCommand>>();
		var fakerDeleteService = Substitute.For<ICommandService<PlayerOnlineDeleteCommand>>();
		var sut = new PlayerOnlineActionCommandService(fakerUpsertService, fakerDeleteService);
		var command = new PlayerOnlineActionCommand(
			GameProviderCode: "KY",
			PlayerCode: "4ucmqplxb029",
			IsUpsert: true,
			Balance: 0,
			FreeBalance: 0,
			OutstandingBalance: 0);

		await sut.ExecuteAsync(
				command: command,
				cancellationToken: default)
			.ConfigureAwait(true);

		_ = fakerUpsertService
			.Received(0)
			.ExecuteAsync(
				command: Arg.Is<PlayerOnlineUpsertCommand>(cmd =>
					cmd.GameProviderCode == "KY" &&
					cmd.PlayerCode == "4ucmqplxb029" &&
					cmd.Balance == 0 &&
					cmd.FreeBalance == 0 &&
					cmd.OutstandingBalance == 0),
				cancellationToken: default);

		_ = fakerDeleteService
			.Received(1)
			.ExecuteAsync(
				command: Arg.Is<PlayerOnlineDeleteCommand>(cmd =>
					cmd.GameProviderCode == "KY" &&
					cmd.PlayerCode == "4ucmqplxb029"),
				cancellationToken: default);
	}
}
