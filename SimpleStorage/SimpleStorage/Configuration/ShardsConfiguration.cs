using System.Net;
using System.Collections.Generic;
using System.Configuration;

namespace SimpleStorage.Configuration
{
	[ConfigurationCollection(typeof(ShardConfiguration))]
	public class ShardsConfiguration : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new ShardConfiguration();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((ShardConfiguration)(element)).Name;
		}
	}
}