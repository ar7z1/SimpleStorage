using System;
using NUnit.Framework;
using Client;
using SimpleStorage;
using System.Net;
using Domain;

namespace Controllers
{
    [TestFixture]
    public class ConfigurationControllerFunctionalTests
    {
        private const int port = 15000;
        private SimpleStorageConfigurationClient configurationClient;
        private SimpleStorageConfiguration configuration;

        [SetUp]
        public void SetUp()
        {
            var endpoint = new IPEndPoint(IPAddress.Loopback, port);
            configurationClient = new SimpleStorageConfigurationClient(endpoint);
            configuration = new SimpleStorageConfiguration(port);
        }

        [Test]
        public void Get_Always_ShouldReturn()
        {
            using (SimpleStorageService.Start(configuration))
            {
                var actual = configurationClient.GetConfiguration();
                Assert.That(actual.Port, Is.EqualTo(port));
            }
        }

        [Test]
        public void AddShard_Always_ShouldAdd()
        {
            var shardEndpoint = new IPEndPoint(IPAddress.Loopback, 15001);

            using (SimpleStorageService.Start(configuration))
            {
                configurationClient.AddShardNode(shardEndpoint);

                var actual = configurationClient.GetConfiguration();
                Assert.That(actual.Shards, Is.EqualTo(new[]{ shardEndpoint }));
            }
        }
    }
}