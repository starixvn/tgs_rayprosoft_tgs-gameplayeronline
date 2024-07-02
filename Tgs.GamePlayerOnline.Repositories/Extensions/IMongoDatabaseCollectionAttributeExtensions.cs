using MongoDB.Driver;

namespace Tgs.MongoDB.Extensions.Attributes;

public static class IMongoDatabaseCollectionAttributeExtensions
{
	/// <summary>
	/// 嘗試使用以 <see cref="CollectionAttribute.Name" /> 屬性作為 collection name 使用,
	/// <para/>否則使用 <see cref="TModel" /> 的 type name
	/// </summary>
	public static IMongoCollection<TModel> GetCollection<TModel>(this IMongoDatabase database)
		=> database.GetCollection<TModel>(CollectionAttribute.GetName<TModel>());
}
