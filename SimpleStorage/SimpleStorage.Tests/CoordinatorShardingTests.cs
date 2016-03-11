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
    [Explicit("Sharding. Coordinator")]
	public class CoordinatorShardingTests
	{
        private static IPEndPoint coordinator = new IPEndPoint(IPAddress.Loopback, 15000);
        private static IPEndPoint shard1 = new IPEndPoint(IPAddress.Loopback, 15001);
        private static IPEndPoint shard2 = new IPEndPoint(IPAddress.Loopback, 15002);
        private static IPEndPoint shard3 = new IPEndPoint(IPAddress.Loopback, 15003);

        private CoordinatorClient sut;
        private IPEndPoint[][] coordinatorTopology;

		[SetUp]
		public void SetUp()
		{
            coordinatorTopology = new[]{ new[]{ shard1 }, new[]{ shard2 }, new[]{ shard3 } };
            sut = new CoordinatorClient(coordinator);
		}

        [Test]
        public void Coordinator_SameId_ShouldReturnSameShard()
        {
            var id = Guid.NewGuid().ToString();

            using (SimpleStorageService.Start(new SimpleStorageConfiguration(coordinator.Port, coordinatorTopology)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(shard1.Port)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(shard2.Port)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(shard3.Port)))
            {
                Assert.IsTrue(sut.Get(id).Equals(sut.Get(id)));
            }
        }

        [Test]
        public void Coordinator_DifferentIds_ShouldRouteTo()
        {
            var actualEndpoints = new HashSet<IPEndPoint>();

            using (SimpleStorageService.Start(new SimpleStorageConfiguration(coordinator.Port, coordinatorTopology)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(shard1.Port)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(shard2.Port)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(shard3.Port)))
            {
                for (var i = 0; i < 100; i++) {
                    actualEndpoints.Add(sut.Get(Guid.NewGuid().ToString()).First());
                }
            }

            Assert.That(actualEndpoints.Count, Is.EqualTo(3));
        }
        //todo тесты должны использовать клиента к сторэджу, который должен использовать координатор
	}
}