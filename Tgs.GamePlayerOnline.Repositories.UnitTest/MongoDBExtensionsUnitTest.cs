using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using NSubstitute;
using Tgs.MongoDB.Extensions.Attributes;

namespace Tgs.GamePlayerOnline.Repositories.UnitTest;

[MongoDB.Extensions.Attributes.Collection("my_model")]
[Index("ix_game_provider_code", fieldName: nameof(GameProviderCode))]
[Index("ix_expire_on", fieldName: nameof(ExpireOn), ExpireAfterSeconds = 1)]
public sealed class MyModel
{
	[BsonId]
	[BsonRepresentation(BsonType.Int64)]
	public long Id { get; set; }

	[BsonElement("game_provider_code")]
	public string GameProviderCode { get; set; } = default!;

	[BsonElement("expire_on")]
	[BsonIgnoreIfNull]
	public DateTime? ExpireOn { get; set; }
}

public sealed class MongoDBExtensionsUnitTest
{
	[Fact]
	public void CollectionAttribute()
	{
		var fakerMongoDatabase = Substitute.For<IMongoDatabase>();

		fakerMongoDatabase.GetCollection<MyModel>();

		fakerMongoDatabase
			.Received(1)
			.GetCollection<MyModel>(name: Arg.Is("my_model"));
	}

	[Fact]
	public async Task IndexAttribute()
	{
		var fakerMongoDatabase = Substitute.For<IMongoDatabase>();
		var fakerCollection = Substitute.For<IMongoCollection<MyModel>>();

		fakerMongoDatabase
			.GetCollection<MyModel>(name: "my_model")
			.Returns(fakerCollection);

		var collection = fakerMongoDatabase.GetCollection<MyModel>("my_model");
		await collection.CreateIndicesAsync().ConfigureAwait(true);

		_ = fakerCollection
			.Indexes
			.Received(1)
			.CreateOneAsync(
				model: Arg.Is<CreateIndexModel<MyModel>>(m =>
					m.Options.Name == "ix_game_provider_code" &&
					m.Options.ExpireAfter == null),
				options: null,
				cancellationToken: default);

		_ = fakerCollection
			.Indexes
			.Received(1)
			.CreateOneAsync(
				model: Arg.Is<CreateIndexModel<MyModel>>(m =>
					m.Options.Name == "ix_expire_on" &&
					m.Options.ExpireAfter == TimeSpan.FromSeconds(1)),
				options: null,
				cancellationToken: default);
	}
}
