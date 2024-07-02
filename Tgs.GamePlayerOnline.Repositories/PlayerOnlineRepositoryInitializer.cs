using Microsoft.Extensions.Hosting;
using Tgs.GamePlayerOnline.Core;

namespace Tgs.GamePlayerOnline.Repositories;

internal class PlayerOnlineRepositoryInitializer(IPlayerOnlineRepository repository) : IHostedService
{
	public async Task StartAsync(CancellationToken cancellationToken)
		=> await repository.InitializeAsync(cancellationToken).ConfigureAwait(false);

	public Task StopAsync(CancellationToken cancellationToken)
		=> Task.CompletedTask;
}