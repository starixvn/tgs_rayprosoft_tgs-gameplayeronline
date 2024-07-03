using MongoDB.Driver;

namespace Tgs.MongoDB.Extensions.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class IndexAttribute(string indexName, string fieldName) : Attribute
{
	public string IndexName { get; init; } = indexName;

	public string FieldName { get; init; } = fieldName;

	/// <summary>
	/// default(0) is not set
	/// <para/>因為目前 attribute 的建構引數尚不支持 'TimeSpan?' 類型
	/// </summary>
	public double ExpireAfterSeconds { get; init; }

	public static IReadOnlyCollection<IndexAttribute> GetDefines<TModel>() => DefineHoldere<TModel>.Defines;

	public Task<string> CreateAsync<TModel>(IMongoCollection<TModel> collection, CancellationToken cancellationToken = default)
	{
		var index = new IndexKeysDefinitionBuilder<TModel>().Ascending(new StringFieldDefinition<TModel>(FieldName));

		var createIndexOptions = new CreateIndexOptions { Name = IndexName };
		if (ExpireAfterSeconds != 0)
			createIndexOptions.ExpireAfter = TimeSpan.FromSeconds(ExpireAfterSeconds);

		var createIndexModel = new CreateIndexModel<TModel>(index, createIndexOptions);

		return collection.Indexes.CreateOneAsync(
			model: createIndexModel,
			cancellationToken: cancellationToken);
	}

	private static class DefineHoldere<TModel>
	{
		public static IReadOnlyCollection<IndexAttribute> Defines => GetDefines();

		private static IReadOnlyCollection<IndexAttribute> GetDefines()
			=> GetCustomAttributes(typeof(TModel), typeof(IndexAttribute)) is { Length: > 0 } attrs
				? attrs.Cast<IndexAttribute>().ToList()
				: Array.Empty<IndexAttribute>();
	}
}
