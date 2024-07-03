using MongoDB.Driver;

namespace Tgs.MongoDB.Extensions.Attributes;

public static class IMongoCollectionIndexAttributeExtensions
{
	public static async ValueTask CreateIndicesAsync<TModel>(this IMongoCollection<TModel> collection, CancellationToken cancellationToken = default)
	{
		foreach (var define in IndexAttribute.GetDefines<TModel>())
			_ = await define.CreateAsync(collection, cancellationToken).ConfigureAwait(false);
	}
}
