using System;
using System.Net;
using Client;
using Domain;
using NUnit.Framework;
using SimpleStorage;

namespace Controllers
{
    [TestFixture]
    public class CoordinatorControllerFunctionalTests
    {
        [SetUp]
        public void SetUp()
        {
            var endpoint = new IPEndPoint(IPAddress.Loopback, port);
            coordinatorClient = new CoordinatorClient(endpoint);
            configuration = new SimpleStorageConfiguration(port, new[] {new[] {endpoint}});
        }

        private const int port = 15000;
        private CoordinatorClient coordinatorClient;
        private SimpleStorageConfiguration configuration;

        [Test]
        public void Get_Always_ShouldReturn()
        {
            using (SimpleStorageService.Start(configuration))
            {
                var actual = coordinatorClient.Get(Guid.NewGuid().ToString());
                Assert.That(actual, Is.EqualTo(new[] {new IPEndPoint(IPAddress.Loopback, port)}));
            }
        }
    }
}