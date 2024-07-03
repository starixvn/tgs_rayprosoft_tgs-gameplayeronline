using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GamePlayerOnline.Client;
using Tgs.GamePlayerOnline.Messages;
using Valhalla.MessageQueue;

namespace Tgs.GamePlayerOnline.IntegrationTest;

public class GamePlayerOnlineClientUnitTest
{
	[Fact]
	public async Task PlayerOnline_Upsert()
	{
		var gameProviderCode = "KY";
		var playerCode = "4ucmqplxb029";
		var balance = 100.0m;
		var freeBalance = 99.0m;
		var outstandingBalance = 1.0m;

		var web = new GamePlayerOnlineApplication(builder => _ = builder.ConfigureServices(services => _ = services.AddTransient(sp =>
			{
				var fakeMessageSender = Substitute.For<IMessageSender>();

				_ = fakeMessageSender.PublishAsync(
						Arg.Is("game.gameplayeronline.upsert.request"),
						Arg.Is(new Protos.PlayerOnlineUpsertRequest
						{
							GameProviderCode = gameProviderCode,
							PlayerCode = playerCode,
							Balance = balance,
							FreeBalance = freeBalance,
							OutstandingBalance = outstandingBalance
						}),
						Arg.Any<CancellationToken>())
						.Returns(callInfo =>
						{
							var data = callInfo.Arg<Protos.PlayerOnlineUpsertRequest>();
							var ct = callInfo.Arg<CancellationToken>();

							var handler = ActivatorUtilities.CreateInstance<GamePlayerOnlineUpsertMessageHandler>(sp);

							return handler.HandleAsync("game.gameplayeronline.upsert.request", data, null, cancellationToken: ct);
						});

				return fakeMessageSender;
			})));

		var httpClient = web.CreateClient();

		var sut = new GamePlayerOnlineClient(httpClient);

		await sut.PlayerOnlineUpsertAsync(gameProviderCode, playerCode, balance, freeBalance, outstandingBalance);
	}
}
