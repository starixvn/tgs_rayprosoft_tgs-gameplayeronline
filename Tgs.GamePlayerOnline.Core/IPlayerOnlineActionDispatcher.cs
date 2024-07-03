using Tgs.GamePlayerOnline.Core.ApplicationServices;

namespace Tgs.GamePlayerOnline.Core;

public interface IPlayerOnlineActionDispatcher
{
	ValueTask PushCommandAsync(PlayerOnlineActionCommand command, CancellationToken cancellationToken = default);
}
