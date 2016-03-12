using System.Linq;
using System.Net;
using Client;
using Domain;
using NUnit.Framework;

namespace SimpleStorage.Tests.Controllers
{
    [TestFixture]
    public class OperationLogControllerFunctionalTests
    {
        [SetUp]
        public void SetUp()
        {
            var endpoint = new IPEndPoint(IPAddress.Loopback, port);
            storageClient = new SimpleStorageClient(endpoint);
            operationLogClient = new OperationLogClient(endpoint);
            configuration = new SimpleStorageConfiguration(port);
        }

        private const int port = 15000;
        private OperationLogClient operationLogClient;
        private SimpleStorageClient storageClient;
        private SimpleStorageConfiguration configuration;

        [Test]
        public void Read_Always_ShouldReturnAllOperations()
        {
            const string id = "id";
            var version1 = new Value {Content = "content", IsDeleted = false, Revision = 0};
            using (SimpleStorageService.Start(configuration))
            {
                storageClient.Put(id, version1);
                var version2 = new Value {IsDeleted = true, Revision = 1, Content = "anotherContent"};
                storageClient.Put(id, version2);

                var actual = operationLogClient.Read(0, 100).ToArray();

                Assert.That(actual.Length, Is.EqualTo(2));
                Assert.That(actual[0].Id, Is.EqualTo(id));
                Assert.That(actual[0].Value.Content, Is.EqualTo(version1.Content));
                Assert.That(actual[0].Value.IsDeleted, Is.False);
                Assert.That(actual[1].Id, Is.EqualTo(id));
                Assert.That(actual[1].Value.IsDeleted, Is.True);
            }
        }

        [Test]
        public void Read_BigPosition_ShouldReturnEmpty()
        {
            using (SimpleStorageService.Start(configuration))
            {
                var actual = operationLogClient.Read(1000, 1).ToArray();
                Assert.That(actual.Length, Is.EqualTo(0));
            }
        }

        [Test]
        public void Read_WithSeek_ShouldSkip()
        {
            using (SimpleStorageService.Start(configuration))
            {
                storageClient.Put("id1", new Value {Content = "1"});
                storageClient.Put("id2", new Value {Content = "2"});
                storageClient.Put("id3", new Value {Content = "3"});

                var actual = operationLogClient.Read(1, 1).ToArray();

                Assert.That(actual.Length, Is.EqualTo(1));
            }
        }
    }
}