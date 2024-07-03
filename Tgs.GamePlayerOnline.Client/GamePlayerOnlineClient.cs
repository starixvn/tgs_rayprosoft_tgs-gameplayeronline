using System.Net.Http.Json;
using System.Text.Json;

using System.Text.Json.Serialization;

namespace Tgs.GamePlayerOnline.Client;

internal class GamePlayerOnlineClient(
	HttpClient httpClient) : IGamePlayerOnlineClient
{
	private static readonly JsonSerializerOptions _SerializerOptions = new()
	{
		WriteIndented = false,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		Converters = { new JsonStringEnumConverter() }
	};

	public async ValueTask PlayerOnlineUpsertAsync(string gameProviderCode, string playerCode, decimal balance, decimal freeBalance, decimal outstandingBalance, CancellationToken cancellationToken = default)
	{
		var source = new
		{
			Balance = balance,
			FreeBalance = freeBalance,
			OutstandingBalance = outstandingBalance
		};
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);

		var response = await httpClient.PutAsync(
			requestUri: $"/playeronline/{gameProviderCode}/{playerCode}",
			content: requestContent,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();
	}
}
