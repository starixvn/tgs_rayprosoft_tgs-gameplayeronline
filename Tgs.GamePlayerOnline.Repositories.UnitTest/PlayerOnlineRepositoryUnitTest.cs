using MongoDB.Driver;
using NSubstitute;
using Tgs.GamePlayerOnline.Core;
using Tgs.GamePlayerOnline.Repositories.Models;

namespace Tgs.GamePlayerOnline.Repositories.UnitTest;

public sealed class PlayerOnlineRepositoryUnitTest
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
	public async Task GetLastUpdate()
	{
		var beginUpdateOn = DateTime.Now.Subtract(TimeSpan.FromSeconds(10));

		var builder = Builders<PlayerOnlineModel>.Filter;

		var filter = builder.Gte(x => x.UpdateOn, beginUpdateOn);

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
			.GetLastUpdateAsync(
				beginUpdateOn: beginUpdateOn)
			.ToListAsync()
			.ConfigureAwait(true);
	}

	[Fact]
	public async Task Add()
	{
		var fakerMongoDatabase = Substitute.For<IMongoDatabase>();

		var gameProviderCode = "KY";
		var brandCode = "GPI";
		var playerCode = "4ucmqplxb029";

		var sut = new PlayerOnlineRepository(
			database: fakerMongoDatabase);

		await sut
			.AddAsync(
				gameProviderCode: gameProviderCode,
				brandCode: brandCode,
				playerCode: playerCode,
				username: playerCode,
				balance: 10,
				freeBalance: 3,
				outstandingBalance: 2)
			.ConfigureAwait(true);

		var document = new PlayerOnlineModel
		{
			GameProviderCode = gameProviderCode,
			BrandCode = brandCode,
			PlayerCode = playerCode,
			Username = playerCode,
			Balance = 10,
			FreeBalance = 3,
			OutstandingBalance = 2,
			UpdateOn = DateTime.UtcNow,
			ExpireOn = null
		};

		_ = fakerMongoDatabase.Received(1)
			.GetCollection<PlayerOnlineModel>(
				name: Arg.Is("player_onlines"))
			.InsertOneAsync(
				document: Arg.Is(document));
	}

	[Fact]
	public async Task Update()
	{
		var fakerMongoDatabase = Substitute.For<IMongoDatabase>();

		var gameProviderCode = "KY";
		var playerCode = "4ucmqplxb029";

		var sut = new PlayerOnlineRepository(
			database: fakerMongoDatabase);

		await sut
			.UpdateAsync(
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

		var update = Builders<PlayerOnlineModel>.Update
			.Set(m => m.Balance, 18)
			.Set(m => m.FreeBalance, 6)
			.Set(m => m.OutstandingBalance, 8)
			.Set(m => m.UpdateOn, DateTime.UtcNow);

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