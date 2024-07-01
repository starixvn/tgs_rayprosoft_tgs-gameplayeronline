using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Tgs.MongoDB.Extensions.Attributes;

namespace Tgs.GamePlayerOnline.Repositories.Models;

[Collection("player_onlines")]
[Index("ix_game_provider_code", fieldName: nameof(GameProviderCode))]
[Index("ix_brand_code", fieldName: nameof(BrandCode))]
[Index("ix_player_code", fieldName: nameof(PlayerCode))]
[Index("ix_update_on", fieldName: nameof(UpdateOn), ExpireAfterSeconds = 1)]
[Index("ix_expire_on", fieldName: nameof(ExpireOn), ExpireAfterSeconds = 1)]
public record class PlayerOnlineModel
{
	[BsonId]
	[BsonRepresentation(BsonType.Int64)]
	public long Id { get; set; }

	[BsonElement("game_provider_code")]
	public string GameProviderCode { get; set; } = default!;

	[BsonElement("brand_code")]
	public string BrandCode { get; set; } = default!;

	[BsonElement("player_code")]
	public string PlayerCode { get; set; } = default!;

	[BsonElement("username")]
	public string Username { get; set; } = default!;

	[BsonElement("balance")]
	[BsonRepresentation(BsonType.Decimal128)]
	public decimal Balance { get; set; }

	[BsonElement("freeBalance")]
	[BsonRepresentation(BsonType.Decimal128)]
	public decimal FreeBalance { get; set; }

	[BsonElement("outstandingBalance")]
	[BsonRepresentation(BsonType.Decimal128)]
	public decimal OutstandingBalance { get; set; }

	[BsonElement("update_on")]
	public DateTime UpdateOn { get; set; }

	[BsonElement("expire_on")]
	[BsonIgnoreIfNull]
	public DateTime? ExpireOn { get; set; }
}
