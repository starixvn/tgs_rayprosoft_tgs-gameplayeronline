namespace Tgs.MongoDB.Extensions.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class CollectionAttribute(string name) : Attribute
{
	public string Name { get; init; } = name;

	public static string GetName<TModel>() => NameHolder<TModel>.Name;

	private static class NameHolder<TModel>
	{
		public static string Name => GetName();

		private static string GetName()
		{
			Type type = typeof(TModel);
			return GetCustomAttribute(type, typeof(CollectionAttribute)) is CollectionAttribute attr ? attr.Name : type.Name;
		}
	}
}
