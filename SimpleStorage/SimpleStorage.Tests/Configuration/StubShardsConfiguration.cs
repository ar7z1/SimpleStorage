using System;
using SimpleStorage.Configuration;
using System.Collections.Generic;
using System.Net;

namespace Configuration
{
	public class StubShardsConfiguration : IShardsConfiguration
	{
		public IEnumerable<IPEndPoint> Shards { get; set; }
	}
}
