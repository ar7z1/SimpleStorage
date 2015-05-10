using System.Net;
using System.Collections.Generic;

namespace SimpleStorage
{
	public interface IShardingConfiguration
	{
		IEnumerable<IPEndPoint> Shards { get; }
	}
}