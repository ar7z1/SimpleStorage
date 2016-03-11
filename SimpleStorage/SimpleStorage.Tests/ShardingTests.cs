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
    [Explicit("Шардинг. Умный клиент")]
	public class ShardingTests
	{
		private const int port1 = 15000;
        private static IPEndPoint shard1 = new IPEndPoint(IPAddress.Loopback, port1);
        private SimpleStorageClient shard1Client;
        private SimpleStorageConfigurationClient configurationClient1;

		private const int port2 = 15001;
        private static IPEndPoint shard2 = new IPEndPoint(IPAddress.Loopback, port2);
        private SimpleStorageClient shard2Client;
        private SimpleStorageConfigurationClient configurationClient2;

		private const int port3 = 15002;
        private static IPEndPoint shard3 = new IPEndPoint(IPAddress.Loopback, port3);
        private SimpleStorageClient shard3Client;
        private SimpleStorageConfigurationClient configurationClient3;

        private SimpleStorageClient sut;

		[SetUp]
		public void SetUp()
		{
			sut = new SimpleStorageClient(new[] { shard1, shard2, shard3 });
            shard1Client = new SimpleStorageClient(shard1);
            configurationClient1 = new SimpleStorageConfigurationClient(shard1);
            shard2Client = new SimpleStorageClient(shard2);
            configurationClient2 = new SimpleStorageConfigurationClient(shard2);
            shard3Client = new SimpleStorageClient(shard3);
            configurationClient3 = new SimpleStorageConfigurationClient(shard3);
		}

        [Test]
        public void Sharding_Only_OnClient()
        {
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port1)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port2)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port3)))
            {
                Assert.That(configurationClient1.GetConfiguration().Shards, Is.Empty);
                Assert.That(configurationClient2.GetConfiguration().Shards, Is.Empty);
                Assert.That(configurationClient3.GetConfiguration().Shards, Is.Empty);
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
                sut.Put(id, new Value { Content = "content" });

                Value value;
                var actual = TryGet(shard1Client, id, out value) && TryGet(shard2Client, id, out value) && TryGet(shard3Client, id, out value);
                Assert.IsFalse(actual, "Данные лежат на всех шардах");
			}
		}

        [Test]
        public void Sharding_AllShards_ShouldContainSomeData()
        {
            var ids = new List<string>();
            for (var i = 0; i < 100; i++) {
                ids.Add(Guid.NewGuid().ToString());
            }

            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port1)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port2)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port3)))
            {
                for (var i = 0; i < ids.Count; i++)
                    sut.Put(ids[i], new Value { Content = "content" });

                var shard1ContainsData = false;
                var shard2ContainsData = false;
                var shard3ContainsData = false;
                Value value;
                for (var i = 0; i < ids.Count; i++) {
                    shard1ContainsData |= TryGet(shard1Client, ids[i], out value);
                    shard2ContainsData |= TryGet(shard2Client, ids[i], out value);
                    shard3ContainsData |= TryGet(shard3Client, ids[i], out value);
                }

                Assert.IsTrue(shard1ContainsData && shard2ContainsData && shard3ContainsData, "Данные распределяются не по всем шардам");
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

        private bool TryGet(SimpleStorageClient client, string id, out Value value) {
            value = null;
            try
            {
                value = shard3Client.Get(id);
                return true;
            }
            catch(HttpRequestException e) {
                if (!e.Message.Contains("404")) {
                    throw;
                }
            }

            return false;
        }
	}
}