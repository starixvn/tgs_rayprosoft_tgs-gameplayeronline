using MongoDB.Driver;
using Tgs.GamePlayerOnline.Core;
using Tgs.GamePlayerOnline.Core.Models;
using Tgs.MongoDB.Extensions.Attributes;
using Tgs.GamePlayerOnline.Repositories.Models;

namespace Tgs.GamePlayerOnline.Repositories;

internal sealed class PlayerOnlineRepository(
	IMongoDatabase database,
	ISnowflakeWork snowflake)
	: IPlayerOnlineRepository
{
	public ValueTask InitializeAsync(CancellationToken cancellationToken = default)
		=> database
			.GetCollection<PlayerOnlineModel>()
			.CreateIndicesAsync(cancellationToken);

	public async ValueTask<PlayerOnlineForm?> GetAsync(
		string gameProviderCode,
		string playerCode,
		CancellationToken cancellationToken = default)
	{
		var builder = Builders<PlayerOnlineModel>.Filter;

		var filter = builder.And(
			builder.Eq(x => x.GameProviderCode, gameProviderCode),
			builder.Eq(x => x.PlayerCode, playerCode));

		var options = new FindOptions<PlayerOnlineModel>
		{
			Skip = 0,
			Limit = 1
		};

		var datas = await database
			.GetCollection<PlayerOnlineModel>()
			.FindAsync(
				filter: filter,
				options: options,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var data = await datas
			.FirstOrDefaultAsync(
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return data == null
			? null
			: new(
				Id: data.Id,
				GameProviderCode: data.GameProviderCode,
				BrandCode: data.BrandCode,
				PlayerCode: data.PlayerCode,
				Username: data.Username,
				Balance: data.Balance,
				FreeBalance: data.FreeBalance,
				OutstandingBalance: data.OutstandingBalance,
				UpdateOn: data.UpdateOn,
				ExpireOn: data.ExpireOn);
	}

	public async ValueTask<long> AddAsync(
		string gameProviderCode,
		string brandCode,
		string playerCode,
		string username,
		decimal balance,
		decimal freeBalance,
		decimal outstandingBalance,
		CancellationToken cancellationToken = default)
	{
		var id = snowflake.GetId();

		var document = new PlayerOnlineModel
		{
			Id = id,
			GameProviderCode = gameProviderCode,
			BrandCode = brandCode,
			PlayerCode = playerCode,
			Username = username,
			Balance = balance,
			FreeBalance = freeBalance,
			OutstandingBalance = outstandingBalance,
			UpdateOn = DateTime.UtcNow,
			ExpireOn = null
		};

		await database
			.GetCollection<PlayerOnlineModel>()
			.InsertOneAsync(
				document: document,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return id;
	}

	public async ValueTask<long> DeleteAsync(
		string gameProviderCode,
		string playerCode,
		CancellationToken cancellationToken = default)
	{
		var builder = Builders<PlayerOnlineModel>.Filter;

		var filter = builder.And(
			builder.Eq(x => x.GameProviderCode, gameProviderCode),
			builder.Eq(x => x.PlayerCode, playerCode));

		var result = await database
			.GetCollection<PlayerOnlineModel>()
			.DeleteOneAsync(
				filter: filter,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return result.DeletedCount;
	}
}