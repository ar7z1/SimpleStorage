using System.Net;
using System.Collections.Generic;

namespace SimpleStorage
{
	public class ServerConfiguration : IShardingConfiguration
	{
		public IEnumerable<IPEndPoint> Shards { get; set; }
	}
}