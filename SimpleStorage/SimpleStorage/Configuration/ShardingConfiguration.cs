using System.Net;
using System.Collections.Generic;

namespace SimpleStorage
{
	public class ShardingConfiguration : IShardingConfiguration
	{
		public IEnumerable<IPEndPoint> Shards { get; set; }
	}
}