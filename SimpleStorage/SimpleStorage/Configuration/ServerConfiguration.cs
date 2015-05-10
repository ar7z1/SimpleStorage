using System.Net;
using System.Collections.Generic;
using SimpleStorage.Configuration;

namespace SimpleStorage
{
	public class ServerConfiguration : IShardsConfiguration
	{
		public IEnumerable<IPEndPoint> Shards { get; set; }
	}
}