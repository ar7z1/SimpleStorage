using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Client;
using Domain;
using NUnit.Framework;
using System.Net;

namespace SimpleStorage.Tests
{
	[TestFixture]
    [Explicit("Sharding. Intra database routing")]
    public class IntraDatabaseRoutingShardingTests
	{
        private const int port1 = 15000;
        private static IPEndPoint shard1 = new IPEndPoint(IPAddress.Loopback, port1);
        private OperationLogClient shard1OplogClient;
        private SimpleStorageConfigurationClient configurationClient1;

        private const int port2 = 15001;
        private static IPEndPoint shard2 = new IPEndPoint(IPAddress.Loopback, port2);
        private OperationLogClient shard2OplogClient;
        private SimpleStorageConfigurationClient configurationClient2;

        private const int port3 = 15002;
        private static IPEndPoint shard3 = new IPEndPoint(IPAddress.Loopback, port3);
        private OperationLogClient shard3OplogClient;
        private SimpleStorageConfigurationClient configurationClient3;

        private SimpleStorageClient sut;

        [SetUp]
        public void SetUp()
        {
            sut = new SimpleStorageClient(shard2);
            shard1OplogClient = new OperationLogClient(shard1);
            configurationClient1 = new SimpleStorageConfigurationClient(shard1);
            shard2OplogClient = new OperationLogClient(shard2);
            configurationClient2 = new SimpleStorageConfigurationClient(shard2);
            shard3OplogClient = new OperationLogClient(shard3);
            configurationClient3 = new SimpleStorageConfigurationClient(shard3);
        }

        [Test]
        public void Sharding_EachShard_ShouldNotContainAllData()
        {
            var id = Guid.NewGuid().ToString();
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port1)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port2)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port3)))
            {
                ConfigureShards();

                sut.Put(id, new Value { Content = "content" });

                var actual = shard1OplogClient.Read(0, 1).Any() &&
                             shard2OplogClient.Read(0, 1).Any() &&
                             shard3OplogClient.Read(0, 1).Any();
                Assert.IsFalse(actual, "Данные лежат на всех шардах");
            }
        }

        [Test]
        public void Sharding_AllShards_ShouldContainSomeData()
        {
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port1)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port2)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port3)))
            {
                ConfigureShards();

                for (var i = 0; i < 100; i++)
                    sut.Put(Guid.NewGuid().ToString(), new Value { Content = Guid.NewGuid().ToString() });

                var actual = shard1OplogClient.Read(0, 1).Any() &&
                             shard2OplogClient.Read(0, 1).Any() &&
                             shard3OplogClient.Read(0, 1).Any();

                Assert.IsTrue(actual, "Данные распределяются не по всем шардам");
            }
        }

        [Test]
        public void Sharding_Always_ShouldSaveAndReadAllData()
        {
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port1)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port2)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port3)))
            {
                ConfigureShards();

                var items = new List<KeyValuePair<string, Value>>();
                for (var i = 0; i < 100; i++) {
                    var id = Guid.NewGuid().ToString();
                    var value = new Value { Content = Guid.NewGuid().ToString() };
                    items.Add(new KeyValuePair<string, Value>(id, value));
                    sut.Put(id, value);
                }

                foreach (var item in items) {
                    var actual = sut.Get(item.Key);
                    Assert.That(actual.Content, Is.EqualTo(item.Value.Content));
                }
            }
        }

        [Test]
        public void Get_UnknownIds_ShouldReturnNotFoundForAll()
        {
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port1)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port2)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port3)))
            {
                ConfigureShards();

                for (var i = 0; i < 100; i++) {
                    var id = Guid.NewGuid().ToString();

                    Assert.Throws(Is.TypeOf<HttpRequestException>().And.Property("Message").Contains("404"),
                                  () => sut.Get(id));
                }
            }
        }

        private void ConfigureShards()
        {
            configurationClient1.AddShardNode(shard2);
            configurationClient1.AddShardNode(shard3);

            configurationClient2.AddShardNode(shard1);
            configurationClient2.AddShardNode(shard3);

            configurationClient3.AddShardNode(shard1);
            configurationClient3.AddShardNode(shard2);
        }
	}
}