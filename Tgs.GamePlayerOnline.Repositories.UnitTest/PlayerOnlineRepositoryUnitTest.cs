using MongoDB.Driver;
using NSubstitute;
using Tgs.GamePlayerOnline.Repositories.Models;

namespace Tgs.GamePlayerOnline.Repositories.UnitTest;

public class PlayerOnlineRepositoryUnitTest
{
	[Fact]
	public async Task Get()
	{
		var fakerMongoDatabase = Substitute.For<IMongoDatabase>();

		var gameProviderCode = "KY";
		var playerCode = "4ucmqplxb029";

		var sut = new PlayerOnlineRepository(
			database: fakerMongoDatabase);

		_ = await sut
			.GetAsync(
				gameProviderCode: gameProviderCode,
				playerCode: playerCode)
			.ConfigureAwait(true);

		var builder = Builders<PlayerOnlineModel>.Filter;

		var filter = builder.And(
			builder.Eq(x => x.GameProviderCode, gameProviderCode),
			builder.Eq(x => x.PlayerCode, playerCode));

		var options = new FindOptions<PlayerOnlineModel>
		{
			Skip = 0,
			Limit = 1
		};

		_ = fakerMongoDatabase.Received(1)
			.GetCollection<PlayerOnlineModel>(
				name: Arg.Is("player_onlines"))
			.FindAsync(
				filter: Arg.Is(filter),
				options: Arg.Is(options));
	}

	[Fact]
	public async Task GetUpdatedBefore()
	{
		var lastUpdateOn = DateTime.Now.Subtract(TimeSpan.FromSeconds(10));

		var builder = Builders<PlayerOnlineModel>.Filter;

		var filter = builder.Gte(x => x.UpdateOn, lastUpdateOn);

		var fakerMongoDatabase = Substitute.For<IMongoDatabase>();
		var fakerCollection = Substitute.For<IMongoCollection<PlayerOnlineModel>>();
		var fakerCursor = Substitute.For<IAsyncCursor<PlayerOnlineModel>>();

		fakerMongoDatabase
			.GetCollection<PlayerOnlineModel>("player_onlines")
			.Returns(fakerCollection);
		fakerCollection
			.FindAsync(
				filter: filter,
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(fakerCursor);
		fakerCursor
			.MoveNextAsync(cancellationToken: default)
			.Returns(false);

		var sut = new PlayerOnlineRepository(
			database: fakerMongoDatabase);

		_ = await sut
			.GetUpdatedBeforeAsync(
				lastUpdateOn: lastUpdateOn)
			.ToListAsync()
			.ConfigureAwait(true);
	}

	[Fact]
	public async Task Upsert()
	{
		var fakerMongoDatabase = Substitute.For<IMongoDatabase>();

		var gameProviderCode = "KY";
		var playerCode = "4ucmqplxb029";

		var sut = new PlayerOnlineRepository(
			database: fakerMongoDatabase);

		await sut
			.UpsertAsync(
				gameProviderCode: gameProviderCode,
				playerCode: playerCode,
				balance: 18,
				freeBalance: 6,
				outstandingBalance: 8)
			.ConfigureAwait(true);

		var builder = Builders<PlayerOnlineModel>.Filter;

		var filter = builder.And(
			builder.Eq(x => x.GameProviderCode, gameProviderCode),
			builder.Eq(x => x.PlayerCode, playerCode));

		var now = DateTime.UtcNow;
		var update = Builders<PlayerOnlineModel>.Update
			.Set(m => m.Balance, 18)
			.Set(m => m.FreeBalance, 6)
			.Set(m => m.OutstandingBalance, 8)
			.Set(m => m.UpdateOn, now)
			.Set(m => m.UpdateOn, now.AddDays(1));

		var options = new UpdateOptions
		{
			IsUpsert = true
		};

		_ = fakerMongoDatabase.Received(1)
			.GetCollection<PlayerOnlineModel>(
				name: Arg.Is("player_onlines"))
			.UpdateOneAsync(
				filter: Arg.Is(filter),
				update: Arg.Is(update),
				options: Arg.Is(options));
	}

	[Fact]
	public async Task Delete()
	{
		var fakerMongoDatabase = Substitute.For<IMongoDatabase>();

		var gameProviderCode = "KY";
		var playerCode = "4ucmqplxb029";

		var sut = new PlayerOnlineRepository(
			database: fakerMongoDatabase);

		await sut
			.DeleteAsync(
				gameProviderCode: gameProviderCode,
				playerCode: playerCode)
			.ConfigureAwait(true);

		var builder = Builders<PlayerOnlineModel>.Filter;

		var filter = builder.And(
			builder.Eq(x => x.GameProviderCode, gameProviderCode),
			builder.Eq(x => x.PlayerCode, playerCode));

		_ = fakerMongoDatabase.Received(1)
			.GetCollection<PlayerOnlineModel>(
				name: Arg.Is("player_onlines"))
			.DeleteOneAsync(
				filter: Arg.Is(filter));
	}
}