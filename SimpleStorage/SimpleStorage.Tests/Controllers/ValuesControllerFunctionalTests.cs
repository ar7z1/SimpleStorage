using System.Net;
using System.Net.Http;
using Client;
using Domain;
using NUnit.Framework;

namespace SimpleStorage.Tests.Controllers
{
	[TestFixture]
	public class ValuesControllerFunctionalTests
	{
		private const int port = 15000;
		private SimpleStorageClient simpleStorageClient;
		private ServiceClient serviceClient;
        private SimpleStorageConfiguration configuration;

		[SetUp]
		public void SetUp()
		{
            var endpoint = new IPEndPoint(IPAddress.Loopback, port);
			simpleStorageClient = new SimpleStorageClient(endpoint);
			serviceClient = new ServiceClient(endpoint);
            configuration = new SimpleStorageConfiguration(port);
		}

		[Test]
		public void Get_KnownId_ShouldReturnValue()
		{
			const string id = "id";
			var value = new Value { Content = "content" };

            using (SimpleStorageService.Start(configuration)) {
				simpleStorageClient.Put(id, value);
				var actual = simpleStorageClient.Get(id);
				Assert.That(actual.Content, Is.EqualTo(value.Content));
			}
		}

		[Test]
		public void Get_StoppedInstance_ShouldThrow()
		{
            using (SimpleStorageService.Start(configuration)) {
				serviceClient.Stop();

				Assert.Throws(Is.TypeOf<HttpRequestException>().And.Property("Message").Contains("500"),
				              () => simpleStorageClient.Get("unknownId"));
			}
		}

		[Test]
		public void Get_StartInstance_ShouldNotThrow()
		{
            using (SimpleStorageService.Start(configuration)) {
				simpleStorageClient.Put("id", new Value());

				serviceClient.Stop();
				serviceClient.Start();
				Assert.DoesNotThrow(() => simpleStorageClient.Get("id"));
			}
		}

		[Test]
		public void Get_UnknownId_ShouldReturnNotFound()
		{
            using (SimpleStorageService.Start(configuration)) {
                Assert.Throws(Is.TypeOf<HttpRequestException>().And.Property("Message").Contains("404"),
                  () => simpleStorageClient.Get("unknownId"));
			}
		}

        [Test]
        public void Put_StoppedInstance_ShouldThrow()
        {
            using (SimpleStorageService.Start(configuration)) {
                serviceClient.Stop();

                Assert.Throws(Is.TypeOf<HttpRequestException>().And.Property("Message").Contains("500"),
                              () => simpleStorageClient.Put("id", new Value {Content = "content"}));
            }
        }
	}
}