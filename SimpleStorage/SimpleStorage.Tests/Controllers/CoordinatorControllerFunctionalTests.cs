using System;
using NUnit.Framework;
using Client;
using SimpleStorage;
using System.Net;
using Domain;

namespace Controllers
{
    [TestFixture]
    public class CoordinatorControllerFunctionalTests
    {
        private const int port = 15000;
        private CoordinatorClient coordinatorClient;
        private SimpleStorageConfiguration configuration;

        [SetUp]
        public void SetUp()
        {
            var endpoint = new IPEndPoint(IPAddress.Loopback, port);
            coordinatorClient = new CoordinatorClient(endpoint);
            configuration = new SimpleStorageConfiguration(port);
        }

        [Test]
        public void Get_Always_ShouldReturn()
        {
            using (SimpleStorageService.Start(configuration))
            {
                var actual = coordinatorClient.Get(Guid.NewGuid().ToString());
                Assert.That(actual, Is.EqualTo(new IPEndPoint(IPAddress.Loopback, port)));
            }
        }
    }
}