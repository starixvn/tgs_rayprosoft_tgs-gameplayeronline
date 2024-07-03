using Tgs.GamePlayerOnline.Core.ApplicationServices;

namespace Tgs.GamePlayerOnline.Core;

public interface IPlayerOnlineCleanDispatcher
{
	ValueTask PushCommandAsync(PlayerOnlineCleanCommand command, CancellationToken cancellationToken = default);
}
