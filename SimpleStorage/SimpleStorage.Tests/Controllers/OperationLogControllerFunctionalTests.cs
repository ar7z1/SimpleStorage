using System.Linq;
using Client;
using Domain;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using Ploeh.AutoFixture;
using SimpleStorage.Infrastructure;
using SimpleStorage.IoC;
using StructureMap;

namespace SimpleStorage.Tests.Controllers
{
    [TestFixture]
    public class OperationLogControllerFunctionalTests
    {
        [SetUp]
        public void SetUp()
        {
            fixture = new Fixture();
            storageClient = new SimpleStorageClient(endpoint);
            operationLogClient = new OperationLogClient(endpoint);
            Container container = IoCFactory.GetContainer();
            container.Configure(c => c.For<IStateRepository>().Use(new StateRepository()));
            var operationLog = new OperationLog();
            container.Configure(c => c.For<IOperationLog>().Use(operationLog));
            container.Configure(c => c.For<IStorage>().Use(new Storage(operationLog)));
        }

        private const int port = 15000;
        private readonly string endpoint = string.Format("http://127.0.0.1:{0}/", port);
        private Fixture fixture;
        private SimpleStorageClient storageClient;
        private OperationLogClient operationLogClient;

        [Test]
        public void Read_Always_ShouldReturnAllOperations()
        {
            var id = fixture.Create<string>();
            var value = fixture.Create<Value>();
            using (WebApp.Start<Startup>(string.Format("http://+:{0}/", port)))
            {
                storageClient.Put(id, value);
                storageClient.Delete(id);

                Operation[] actual = operationLogClient.Read(0, 100).ToArray();

                Assert.That(actual.Length, Is.EqualTo(2));
                Assert.That(actual[0].Id, Is.EqualTo(id));
                Assert.That(actual[0].Type, Is.EqualTo(OperationType.Put));
                Assert.That(actual[0].Value.Content, Is.EqualTo(value.Content));
                Assert.That(actual[1].Id, Is.EqualTo(id));
                Assert.That(actual[1].Type, Is.EqualTo(OperationType.Delete));
            }
        }

        [Test]
        public void Read_WithSeek_ShouldSkip()
        {
            using (WebApp.Start<Startup>(string.Format("http://+:{0}/", port)))
            {
                storageClient.Put(fixture.Create<string>(), fixture.Create<Value>());
                storageClient.Put(fixture.Create<string>(), fixture.Create<Value>());
                storageClient.Put(fixture.Create<string>(), fixture.Create<Value>());

                Operation[] actual = operationLogClient.Read(1, 1).ToArray();

                Assert.That(actual.Length, Is.EqualTo(1));
            }
        }

        [Test]
        public void Read_BigPosition_ShouldReturnEmpty()
        {
            using (WebApp.Start<Startup>(string.Format("http://+:{0}/", port)))
            {
                Operation[] actual = operationLogClient.Read(1000, 1).ToArray();
                Assert.That(actual.Length, Is.EqualTo(0));
            }
        }
    }
}