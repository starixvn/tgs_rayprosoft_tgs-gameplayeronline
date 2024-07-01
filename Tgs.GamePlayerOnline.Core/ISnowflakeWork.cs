using System.Net;

namespace Tgs.GamePlayerOnline.Core;

public interface ISnowflakeWork
{
	long GetDatacenterId();

	long GetId();

	IPAddress GetIPAddress();

	long GetWorkerId();
}
