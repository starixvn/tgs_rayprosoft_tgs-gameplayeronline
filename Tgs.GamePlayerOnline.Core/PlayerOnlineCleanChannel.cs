using System.Diagnostics;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Tgs.GamePlayerOnline.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GamePlayerOnline.Core;

internal class PlayerOnlineCleanChannel(
	ILogger<PlayerOnlineCleanChannel> logger,
	ICommandService<PlayerOnlineCleanCommand> service) : IDisposable, IAsyncDisposable
{
	private static readonly ActivitySource _ActivitySource = new(name: typeof(PlayerOnlineCleanChannel).FullName!);

	private readonly Channel<PlayerOnlineCleanCommand> _channel = Channel.CreateUnbounded<PlayerOnlineCleanCommand>(
		new UnboundedChannelOptions
		{
			SingleReader = true
		});

	private readonly SemaphoreSlim _locker = new(1, 1);
	private readonly CancellationTokenSource _cancellationTokenSource = new();

	private Task? _backgroundTask;
	private bool _disposedValue;

	~PlayerOnlineCleanChannel() => Dispose(disposing: false);

	public async ValueTask PrepareAsync(CancellationToken cancellationToken = default)
	{
		using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);

		if (_backgroundTask is not null)
			return;

		await _locker.WaitAsync(cts.Token).ConfigureAwait(false);

		try
		{
			if (_backgroundTask is not null)
				return;

			_backgroundTask = Task.Run(
				() => ProcessCommandLoopAsync(_cancellationTokenSource.Token),
				_cancellationTokenSource.Token);
		}
		finally
		{
			_ = _locker.Release();
		}
	}

	public async ValueTask PushCommandAsync(PlayerOnlineCleanCommand command, CancellationToken cancellationToken = default)
	{
		using var cts = CancellationTokenSource.CreateLinkedTokenSource(
			cancellationToken,
			_cancellationTokenSource.Token);

		await _channel.Writer.WriteAsync(
			command,
			cts.Token).ConfigureAwait(false);
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
				_channel.Writer.Complete();
				_cancellationTokenSource.Cancel();
			}

			_cancellationTokenSource.Dispose();
			_disposedValue = true;
		}
	}

	private async Task ProcessCommandLoopAsync(CancellationToken cancellationToken = default)
	{
		await Task.Yield();

		var reader = _channel.Reader;
		while (await reader
			.WaitToReadAsync(cancellationToken)
			.ConfigureAwait(false))
		{
			if (reader.TryRead(out var command))
			{
				using var activity = _ActivitySource.StartActivity(
					"PlayerOnlineCleanChannel handle exchange",
					 ActivityKind.Internal,
					parentContext: default);
				try
				{
					logger.LogInformation("{logInformation}",
						new
						{
							LongOn = DateTime.Now.ToString("o"),
							Data = new
							{
								command.Deadline,
							}
						});

					await service.ExecuteAsync(
						command: command,
						cancellationToken: cancellationToken)
						.ConfigureAwait(false);
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "{logError}",
						new
						{
							LongOn = DateTime.Now.ToString("o"),
							Data = new
							{
								command.Deadline,
							}
						});
				}
			}
		}
	}
}
