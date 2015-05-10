using System;
using NUnit.Framework;
using System.Configuration;
using SimpleStorage.Configuration;
using System.Net;
using System.Linq;

namespace Configuration
{
	[TestFixture]
	public class SimpleStorageConfigurationSectionTests
	{
		[Test]
		public void ConfigurationManager_Always_ShouldReadShardConfigurationFromAppConfig()
		{
			var configuration = (SimpleStorageConfigurationSection)ConfigurationManager.GetSection("simpleStorage");

			var actual = configuration.Shards;

			Assert.That(actual.Single(), Is.EqualTo(new IPEndPoint(IPAddress.Loopback, 15000)));
		}
	}
}

