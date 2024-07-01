using MongoDB.Driver;
using Tgs.GamePlayerOnline.Core;
using Tgs.GamePlayerOnline.Core.Models;
using Tgs.MongoDB.Extensions.Attributes;
using Tgs.GamePlayerOnline.Repositories.Models;
using System.Runtime.CompilerServices;

namespace Tgs.GamePlayerOnline.Repositories;

internal sealed class PlayerOnlineRepository(IMongoDatabase database) : IPlayerOnlineRepository
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

		using var cursor = await database
			.GetCollection<PlayerOnlineModel>()
			.FindAsync(
				filter: filter,
				options: options,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var data = await cursor
			.FirstOrDefaultAsync(
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return data == null
			? null
			: new(
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

	public async IAsyncEnumerable<PlayerOnlineForm> GetLastUpdateAsync(
		DateTime beginUpdateOn,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var builder = Builders<PlayerOnlineModel>.Filter;

		var filter = builder.Gte(x => x.UpdateOn, beginUpdateOn);

		using var cursor = await database
			.GetCollection<PlayerOnlineModel>()
			.FindAsync(
				filter: filter,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
		{
			foreach (var data in cursor.Current)
				yield return new(
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
	}

	public async ValueTask AddAsync(
		string gameProviderCode,
		string brandCode,
		string playerCode,
		string username,
		decimal balance,
		decimal freeBalance,
		decimal outstandingBalance,
		CancellationToken cancellationToken = default)
	{
		var document = new PlayerOnlineModel
		{
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
	}

	public async ValueTask UpdateAsync(
		string gameProviderCode,
		string playerCode,
		decimal balance,
		decimal freeBalance,
		decimal outstandingBalance,
		CancellationToken cancellationToken = default)
	{
		var builder = Builders<PlayerOnlineModel>.Filter;

		var filter = builder.And(
			builder.Eq(x => x.GameProviderCode, gameProviderCode),
			builder.Eq(x => x.PlayerCode, playerCode));

		var now = DateTime.UtcNow;
		var update = Builders<PlayerOnlineModel>.Update
			.Set(m => m.Balance, balance)
			.Set(m => m.FreeBalance, freeBalance)
			.Set(m => m.OutstandingBalance, outstandingBalance)
			.Set(m => m.UpdateOn, now)
			.Set(m => m.ExpireOn, now.AddSeconds(30));

		var options = new UpdateOptions
		{
			IsUpsert = true
		};

		await database
			.GetCollection<PlayerOnlineModel>()
			.UpdateOneAsync(
				filter: filter,
				update: update,
				options: options,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask DeleteAsync(
		string gameProviderCode,
		string playerCode,
		CancellationToken cancellationToken = default)
	{
		var builder = Builders<PlayerOnlineModel>.Filter;

		var filter = builder.And(
			builder.Eq(x => x.GameProviderCode, gameProviderCode),
			builder.Eq(x => x.PlayerCode, playerCode));

		await database
			.GetCollection<PlayerOnlineModel>()
			.DeleteOneAsync(
				filter: filter,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}