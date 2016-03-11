using System;
using System.Linq;
using Client;
using Domain;
using NUnit.Framework;
using System.Net;

namespace SimpleStorage.Tests
{
    [TestFixture]
    [Explicit("Quorum replication")]
    public class QuorumReplicationTests
    {
        private const int port1 = 15000;
        private static IPEndPoint node1 = new IPEndPoint(IPAddress.Loopback, port1);

        private const int port2 = 15001;
        private static IPEndPoint node2 = new IPEndPoint(IPAddress.Loopback, port2);

        private const int port3 = 15002;
        private static IPEndPoint node3 = new IPEndPoint(IPAddress.Loopback, port3);

        private SimpleStorageClient sut;
        private ServiceClient node2ServiceClient;
        private ServiceClient node3ServiceClient;
        private ServiceClient node1ServiceClient;
        private OperationLogClient node1OplogClient;
        private OperationLogClient node2OplogClient;
        private OperationLogClient node3OplogClient;
        private Random random;

        [SetUp]
        public void SetUp()
        {
            var topology = new[] {new[] {node1, node2, node3 }};
            sut = new SimpleStorageClient(topology);
            node1OplogClient = new OperationLogClient(node1);
            node2OplogClient = new OperationLogClient(node2);
            node3OplogClient = new OperationLogClient(node3);
            node1ServiceClient = new ServiceClient(node1);
            node2ServiceClient = new ServiceClient(node2);
            node3ServiceClient = new ServiceClient(node3);
            random = new Random();
        }

        [Test]
        public void Replication_EachValue_ShouldBeWrittenOnSeveralReplicas()
        {
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port1)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port2)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port3)))
            {
                sut.Put(Guid.NewGuid().ToString(), new Value { Content = Guid.NewGuid().ToString() });

                var actual = new[]
                {
                    node1OplogClient.Read(0, 1).Any(),
                    node2OplogClient.Read(0, 1).Any(),
                    node3OplogClient.Read(0, 1).Any()
                };

                Assert.That(actual.Where(b => b).Count(), Is.GreaterThan(1), "Данные сохраняются ненадежно");
            }
        }

        [Test]
        public void Replication_EachReplica_ShouldAcceptWrites()
        {
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port1)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port2)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port3)))
            {
                for (var i = 0; i < 100; i++)
                    sut.Put(Guid.NewGuid().ToString(), new Value { Content = Guid.NewGuid().ToString() });

                var actual = node1OplogClient.Read(0, 1).Any() &&
                             node2OplogClient.Read(0, 1).Any() &&
                             node3OplogClient.Read(0, 1).Any();

                Assert.IsTrue(actual, "Данные распределяются не по всем репликам");
            }
        }

        [Test]
        public void Replication_EachValue_ShouldBeAvaliable_WhenOneReplicaIsDown()
        {
            var id = Guid.NewGuid().ToString();
            var value = new Value{Revision = 1, Content = "Content"};
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port1)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port2)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port3)))
            {
                for (int i = 0; i < 100; i++)
                {
                    var client = GetRandomServiceClient();
                    sut.Put(id, value);
                    client.Stop();
                    var actual = sut.Get(id);
                    client.Start();
                    Assert.That(actual.Revision, Is.EqualTo(value.Revision));
                    value.Revision++;
                }
                for (int i = 0; i < 100; i++)
                {
                    var client = GetRandomServiceClient();
                    client.Stop();
                    sut.Put(id, value);
                    client.Start();
                    var actual = sut.Get(id);
                    Assert.That(actual.Revision, Is.EqualTo(value.Revision));
                    value.Revision++;
                }
                for (int i = 0; i < 100; i++)
                {
                    var client = GetRandomServiceClient();
                    client.Stop();
                    sut.Put(id, value);
                    client.Start();
                    client = GetRandomServiceClient();
                    client.Stop();
                    var actual = sut.Get(id);
                    client.Start();
                    Assert.That(actual.Revision, Is.EqualTo(value.Revision));
                    value.Revision++;
                }
            }
        }

        private ServiceClient GetRandomServiceClient()
        {
            var clients = new[] {node1ServiceClient, node2ServiceClient, node3ServiceClient};
            var position = random.Next(3);
            return clients[position];
        }
    }
}