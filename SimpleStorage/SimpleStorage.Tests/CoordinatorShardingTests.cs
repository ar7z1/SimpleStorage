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
    [Explicit("Sharding. Coordinator")]
    public class CoordinatorShardingTests
    {
        [SetUp]
        public void SetUp()
        {
            coordinatorTopology = new[] {new[] {shard1}, new[] {shard2}, new[] {shard3}};
            sut = new CoordinatorClient(coordinator);
        }

        private readonly IPEndPoint coordinator = new IPEndPoint(IPAddress.Loopback, 15000);
        private readonly IPEndPoint shard1 = new IPEndPoint(IPAddress.Loopback, 15001);
        private readonly IPEndPoint shard2 = new IPEndPoint(IPAddress.Loopback, 15002);
        private readonly IPEndPoint shard3 = new IPEndPoint(IPAddress.Loopback, 15003);

        private CoordinatorClient sut;
        private IPEndPoint[][] coordinatorTopology;

        [Test]
        public void Coordinator_DifferentIds_ShouldRouteToAllShards()
        {
            var actualEndpoints = new HashSet<IPEndPoint>();

            using (SimpleStorageService.Start(new SimpleStorageConfiguration(coordinator.Port, coordinatorTopology)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(shard1.Port)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(shard2.Port)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(shard3.Port)))
            {
                for (var i = 0; i < 100; i++)
                {
                    actualEndpoints.Add(sut.Get(Guid.NewGuid().ToString()).First());
                }
            }

            Assert.That(actualEndpoints.Count, Is.EqualTo(3));
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
                var endpoint1 = sut.Get(id).First();
                var endpoint2 = sut.Get(id).First();

                Assert.IsTrue(endpoint1.Equals(endpoint2));
            }
        }

        [Test]
        public void StorageClient_Always_CanReadAndWriteToNodeFromCoordinator()
        {
            var id = Guid.NewGuid().ToString();
            var value = new Value {Content = Guid.NewGuid().ToString()};

            using (SimpleStorageService.Start(new SimpleStorageConfiguration(coordinator.Port, coordinatorTopology)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(shard1.Port)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(shard2.Port)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(shard3.Port)))
            {
                var endpoint = sut.Get(id).First();
                var storageClient = new SimpleStorageClient(endpoint);
                storageClient.Put(id, value);

                var actual = storageClient.Get(id);

                Assert.That(actual.Content, Is.EqualTo(value.Content));
            }
        }
    }
}