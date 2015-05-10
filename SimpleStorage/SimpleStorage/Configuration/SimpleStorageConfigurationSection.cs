using System.Net;
using System.Collections.Generic;
using System.Configuration;

namespace SimpleStorage.Configuration
{
	public class SimpleStorageConfigurationSection : ConfigurationSection, IShardsConfiguration
	{
		private const string ShardsKeyName = "shards";

		[ConfigurationProperty(ShardsKeyName)]
		private ShardsConfiguration ShardsConfigurations {
			get { return ((ShardsConfiguration)(base[ShardsKeyName])); }
		}

		public IEnumerable<IPEndPoint> Shards {
			get {
				foreach (ShardConfiguration shardConfig in ShardsConfigurations) {
					yield return shardConfig.IPEndpoint;
				}
			}
		}
	}
}