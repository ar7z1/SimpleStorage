using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Client;
using Domain;
using NUnit.Framework;

namespace SimpleStorage.Tests
{
    [TestFixture]
    [Explicit("Sharding. Smart client")]
    public class SmartClientShardingTests
    {
        [SetUp]
        public void SetUp()
        {
            var clientTopology = new[] {new[] {shard1}, new[] {shard2}, new[] {shard3}};
            sut = new SimpleStorageClient(clientTopology);
            shard1OplogClient = new OperationLogClient(shard1);
            shard2OplogClient = new OperationLogClient(shard2);
            shard3OplogClient = new OperationLogClient(shard3);
        }

        private const int port1 = 15000;
        private readonly IPEndPoint shard1 = new IPEndPoint(IPAddress.Loopback, port1);
        private OperationLogClient shard1OplogClient;

        private const int port2 = 15001;
        private readonly IPEndPoint shard2 = new IPEndPoint(IPAddress.Loopback, port2);
        private OperationLogClient shard2OplogClient;

        private const int port3 = 15002;
        private readonly IPEndPoint shard3 = new IPEndPoint(IPAddress.Loopback, port3);
        private OperationLogClient shard3OplogClient;

        private SimpleStorageClient sut;

        [Test]
        public void Sharding_AllShards_ShouldContainSomeData()
        {
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port1)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port2)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port3)))
            {
                for (var i = 0; i < 100; i++)
                    sut.Put(Guid.NewGuid().ToString(), new Value {Content = Guid.NewGuid().ToString()});

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
                var items = new List<KeyValuePair<string, Value>>();
                for (var i = 0; i < 100; i++)
                {
                    var id = Guid.NewGuid().ToString();
                    var value = new Value {Content = Guid.NewGuid().ToString()};
                    items.Add(new KeyValuePair<string, Value>(id, value));
                    sut.Put(id, value);
                }

                foreach (var item in items)
                {
                    var actual = sut.Get(item.Key);
                    Assert.That(actual.Content, Is.EqualTo(item.Value.Content));
                }
            }
        }

        [Test]
        public void Sharding_EachShard_ShouldNotContainAllData()
        {
            var id = Guid.NewGuid().ToString();
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port1)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port2)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port3)))
            {
                sut.Put(id, new Value {Content = "content"});

                var actual = shard1OplogClient.Read(0, 1).Any() &&
                             shard2OplogClient.Read(0, 1).Any() &&
                             shard3OplogClient.Read(0, 1).Any();
                Assert.IsFalse(actual, "Данные лежат на всех шардах");
            }
        }
    }
}