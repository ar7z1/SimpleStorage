using System.Net;
using System.Collections.Generic;
using System.Configuration;

namespace SimpleStorage.Configuration
{
	public class SimpleStorageConfigurationSection : ConfigurationSection, IShardsConfiguration, IServerConfiguration
	{
		private const string ShardsKeyName = "shards";
		private const string PortKeyName = "port";

		[ConfigurationProperty(ShardsKeyName)]
		private ShardsConfiguration ShardsConfigurations {
			get { return (ShardsConfiguration)base[ShardsKeyName]; }
		}

		[ConfigurationProperty(PortKeyName, IsRequired = true)]
		public int Port {
			get { return (int)(base[PortKeyName]); }
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