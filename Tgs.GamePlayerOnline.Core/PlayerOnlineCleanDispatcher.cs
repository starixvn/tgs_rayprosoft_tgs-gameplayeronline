using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Tgs.GamePlayerOnline.Core.ApplicationServices;

namespace Tgs.GamePlayerOnline.Core;

internal class PlayerOnlineCleanDispatcher : IPlayerOnlineCleanDispatcher, IDisposable, IAsyncDisposable
{
	private readonly PlayerOnlineCleanChannel _channel;
	private readonly CancellationTokenSource _cancellationTokenSource = new();

	private bool _disposedValue;

	public PlayerOnlineCleanDispatcher(IServiceProvider serviceProvider)
	{
		_channel = ActivatorUtilities.CreateInstance<PlayerOnlineCleanChannel>(serviceProvider);
	}

	~PlayerOnlineCleanDispatcher() => Dispose(false);

	public async ValueTask PushCommandAsync(PlayerOnlineCleanCommand command, CancellationToken cancellationToken = default)
	{
		using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);
		await _channel.PrepareAsync(cts.Token).ConfigureAwait(false);
		await _channel.PushCommandAsync(command, cts.Token).ConfigureAwait(false);
	}

	public ValueTask DisposeAsync()
	{
		Dispose(true);
		GC.SuppressFinalize(this);

		return ValueTask.CompletedTask;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				_cancellationTokenSource.Cancel();
			}

			_cancellationTokenSource.Dispose();
			_disposedValue = true;
		}
	}
}
