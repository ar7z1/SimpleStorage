using System.Net;
using System.Collections.Generic;

namespace SimpleStorage.Configuration
{
	public interface IShardsConfiguration
	{
		IEnumerable<IPEndPoint> Shards { get; }
	}

}