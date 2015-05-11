using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Client;
using Domain;
using NUnit.Framework;
using SimpleStorage.Configuration;
using Configuration;
using System.Net;

namespace SimpleStorage.Tests
{
	[TestFixture]
	[Ignore]
	public class ShardingTests
	{
		private const int port1 = 15000;
		private static IPEndPoint EndPoint1 = new IPEndPoint(IPAddress.Loopback, port1);
		private const int port2 = 15001;
		private static IPEndPoint EndPoint2 = new IPEndPoint(IPAddress.Loopback, port2);
		private const int port3 = 15002;
		private static IPEndPoint EndPoint3 = new IPEndPoint(IPAddress.Loopback, port3);
		private SimpleStorageClient simpleStorageClient;
		private readonly IShardsConfiguration shardsConfiguration = new StubShardsConfiguration() { 
			Shards = new[] { EndPoint1, EndPoint2, EndPoint3 }
		};

		[SetUp]
		public void SetUp()
		{
			simpleStorageClient = new SimpleStorageClient(new[] { EndPoint1, EndPoint2, EndPoint3 });
		}

		[Test]
		public void Sharding_EachShard_ShouldNotContainAllData()
		{
			using (SimpleStorageTestHelpers.StartService(port1, shardsConfiguration))
			using (SimpleStorageTestHelpers.StartService(port2, shardsConfiguration))
			using (SimpleStorageTestHelpers.StartService(port3, shardsConfiguration)) {
				for (var i = 0; i < 100; i++)
					simpleStorageClient.Put(Guid.NewGuid().ToString(), new Value { Content = "content" });

				Assert.That(new LocalStorageClient(EndPoint1).GetAllData().ToArray(), Has.Length.LessThan(100));
				Assert.That(new LocalStorageClient(EndPoint2).GetAllData().ToArray(), Has.Length.LessThan(100));
				Assert.That(new LocalStorageClient(EndPoint3).GetAllData().ToArray(), Has.Length.LessThan(100));
			}
		}

		[Test]
		public void Sharding_AllShards_ShouldContainSomeData()
		{
			using (SimpleStorageTestHelpers.StartService(port1, shardsConfiguration))
			using (SimpleStorageTestHelpers.StartService(port2, shardsConfiguration))
			using (SimpleStorageTestHelpers.StartService(port3, shardsConfiguration)) {
				for (var i = 0; i < 100; i++)
					simpleStorageClient.Put(Guid.NewGuid().ToString(), new Value { Content = "content" });

				Assert.That(new LocalStorageClient(EndPoint1).GetAllData().ToArray(), Has.Length.GreaterThan(0));
				Assert.That(new LocalStorageClient(EndPoint2).GetAllData().ToArray(), Has.Length.GreaterThan(0));
				Assert.That(new LocalStorageClient(EndPoint3).GetAllData().ToArray(), Has.Length.GreaterThan(0));
			}
		}

		[Test]
		public void Sharding_Always_ShouldSaveAllData()
		{
			using (SimpleStorageTestHelpers.StartService(port1, shardsConfiguration))
			using (SimpleStorageTestHelpers.StartService(port2, shardsConfiguration))
			using (SimpleStorageTestHelpers.StartService(port3, shardsConfiguration)) {
				var items = new List<KeyValuePair<string, Value>>();
				for (var i = 0; i < 100; i++) {
					var id = Guid.NewGuid().ToString();
					var value = new Value { Content = "content" };
					items.Add(new KeyValuePair<string, Value>(id, value));
					simpleStorageClient.Put(id, value);
				}

				foreach (var item in items) {
					var actual = simpleStorageClient.Get(item.Key);
					Assert.That(actual.Content, Is.EqualTo(item.Value.Content));
					Assert.That(actual.IsDeleted, Is.EqualTo(item.Value.IsDeleted));
					Assert.That(actual.Revision, Is.EqualTo(item.Value.Revision));
				}
			}
		}
	}
}