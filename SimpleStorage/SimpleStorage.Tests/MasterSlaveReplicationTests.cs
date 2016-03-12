using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using Client;
using Domain;
using NUnit.Framework;

namespace SimpleStorage.Tests
{
    [TestFixture]
    [Explicit("Master-Slave replication")]
    public class MasterSlaveReplicationTests
    {
        [SetUp]
        public void SetUp()
        {
            var masterTopology = new[] {new[] {master}};
            var slaveTopology = new[] {new[] {slave1, slave2}};
            masterClient = new SimpleStorageClient(masterTopology);
            slaveClient = new SimpleStorageClient(slaveTopology);
            masterServiceClient = new ServiceClient(master);
            slave1ServiceClient = new ServiceClient(slave1);
            slave2ServiceClient = new ServiceClient(slave2);
        }

        private const int port1 = 15000;
        private static readonly IPEndPoint master = new IPEndPoint(IPAddress.Loopback, port1);

        private const int port2 = 15001;
        private static readonly IPEndPoint slave1 = new IPEndPoint(IPAddress.Loopback, port2);

        private const int port3 = 15002;
        private static readonly IPEndPoint slave2 = new IPEndPoint(IPAddress.Loopback, port3);

        private SimpleStorageClient masterClient;
        private SimpleStorageClient slaveClient;
        private ServiceClient slave1ServiceClient;
        private ServiceClient slave2ServiceClient;
        private ServiceClient masterServiceClient;

        [Test]
        public void Replication_EachNode_ShouldAcceptReads()
        {
            var id = Guid.NewGuid().ToString();
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port1)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port2) {Master = master}))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port3) {Master = master}))
            {
                Assert.Throws(Is.TypeOf<HttpRequestException>().And.Property("Message").Contains("404"),
                    () => masterClient.Get(id));
                Assert.Throws(Is.TypeOf<HttpRequestException>().And.Property("Message").Contains("404"),
                    () => slaveClient.Get(id));
            }
        }

        [Test]
        public void Replication_EachNode_ShouldContainAllDataEventually()
        {
            var id = Guid.NewGuid().ToString();
            var value = new Value {Content = "content"};
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port1)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port2) {Master = master}))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port3) {Master = master}))
            {
                masterClient.Put(id, value);
                Thread.Sleep(2000);
                var valueFromMaster = masterClient.Get(id);
                Assert.That(valueFromMaster.Content, Is.EqualTo(value.Content));
                var valueFromSlave = slaveClient.Get(id);
                Assert.That(valueFromSlave.Content, Is.EqualTo(value.Content));
            }
        }

        [Test]
        public void Replication_Master_ShouldAcceptReadsAndWrites_WhenSlavesAreDown()
        {
            var id = Guid.NewGuid().ToString();
            var value = new Value {Content = "content"};
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port1)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port2) {Master = master}))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port3) {Master = master}))
            {
                slave1ServiceClient.Stop();
                slave2ServiceClient.Stop();
                masterClient.Put(id, value);
                var valueFromMaster = masterClient.Get(id);
                Assert.That(valueFromMaster.Content, Is.EqualTo(value.Content));
            }
        }

        [Test]
        public void Replication_OnlyMaster_ShouldAcceptWrites()
        {
            var id = Guid.NewGuid().ToString();
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port1)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port2) {Master = master}))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port3) {Master = master}))
            {
                Assert.Throws(Is.TypeOf<HttpRequestException>().And.Property("Message").Contains("403"),
                    () => slaveClient.Put(id, new Value {Content = "content"}));
                masterClient.Put(id, new Value {Content = "content"});
            }
        }

        [Test]
        public void Replication_Slaves_ShouldAcceptReads_WhenMasterIsDown()
        {
            var id = Guid.NewGuid().ToString();
            var value = new Value {Content = "content"};
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port1)))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port2) {Master = master}))
            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port3) {Master = master}))
            {
                masterClient.Put(id, value);
                Thread.Sleep(2000);
                masterServiceClient.Stop();
                var valueFromSlave = slaveClient.Get(id);
                Assert.That(valueFromSlave.Content, Is.EqualTo(value.Content));
            }
        }
    }
}