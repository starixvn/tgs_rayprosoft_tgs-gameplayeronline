using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Tgs.GamePlayerOnline;

internal class MongoDBConnectionManager
{
	public IMongoDatabase MongoDatabase { get; }

	public MongoDBConnectionManager(IOptions<MongoDBOptions> optionsAccessor)
	{
		var options = optionsAccessor.Value;
		var mongoUrl = new MongoUrl(options.ConnectionString);
		MongoDatabase = new MongoClient(mongoUrl)
			.GetDatabase("game_player_online_master");
	}
}
