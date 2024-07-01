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
		var fakerSnowflake = Substitute.For<ISnowflakeWork>();

		var gameProviderCode = "KY";
		var playerCode = "4ucmqplxb029";

		var sut = new PlayerOnlineRepository(
			database: fakerMongoDatabase,
			snowflake: fakerSnowflake);

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
	public async Task Add()
	{
		var fakerMongoDatabase = Substitute.For<IMongoDatabase>();
		var fakerSnowflake = Substitute.For<ISnowflakeWork>();

		var gameProviderCode = "KY";
		var brandCode = "GPI";
		var playerCode = "4ucmqplxb029";

		var formId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		_ = fakerSnowflake.GetId().Returns(formId);

		var sut = new PlayerOnlineRepository(
			database: fakerMongoDatabase,
			snowflake: fakerSnowflake);

		_ = await sut
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
			Id = formId,
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
	public async Task Delete()
	{
		var fakerMongoDatabase = Substitute.For<IMongoDatabase>();
		var fakerSnowflake = Substitute.For<ISnowflakeWork>();

		var gameProviderCode = "KY";
		var playerCode = "4ucmqplxb029";

		var sut = new PlayerOnlineRepository(
			database: fakerMongoDatabase,
			snowflake: fakerSnowflake);

		_ = await sut
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