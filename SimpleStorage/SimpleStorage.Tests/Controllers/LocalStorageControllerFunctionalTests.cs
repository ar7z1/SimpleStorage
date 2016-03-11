using System.Net;
using System.Net.Http;
using Client;
using Domain;
using NUnit.Framework;
using System.Linq;

namespace SimpleStorage.Tests.Controllers
{
	[TestFixture]
	public class LocalStorageControllerFunctionalTests
	{
		private const int port = 15000;
		private LocalStorageClient localStorageClient;
		private ServiceClient serviceClient;
        private SimpleStorageConfiguration configuration;

		[SetUp]
		public void SetUp()
		{
            var endpoint = new IPEndPoint(IPAddress.Loopback, port);
            localStorageClient = new LocalStorageClient(endpoint);
            serviceClient = new ServiceClient(endpoint);
            configuration = new SimpleStorageConfiguration(port);
		}

		[Test]
		public void Get_KnownId_ShouldReturnValue()
		{
			const string id = "id";
			var value = new Value { Content = "content" };

            using (SimpleStorageService.Start(configuration)) {
				localStorageClient.Put(id, value);
				var actual = localStorageClient.Get(id);
				Assert.That(actual.Content, Is.EqualTo(value.Content));
			}
		}

		[Test]
		public void Get_UnknownId_ShouldReturnNotFound()
		{
            using (SimpleStorageService.Start(configuration)) {
				Assert.Throws(Is.TypeOf<HttpRequestException>().And.Property("Message").Contains("404"),
				              () => localStorageClient.Get("unknownId"));
			}
		}

		[Test]
		public void Get_StoppedInstance_ShouldReturnInternalServerError()
		{
            using (SimpleStorageService.Start(configuration)) {
				serviceClient.Stop();

				Assert.Throws(Is.TypeOf<HttpRequestException>().And.Property("Message").Contains("500"),
				              () => localStorageClient.Get("unknownId"));
			}
		}

		[Test]
		public void Put_StoppedInstance_ShouldReturnInternalServerError()
		{
            using (SimpleStorageService.Start(configuration)) {
				serviceClient.Stop();

				Assert.Throws(Is.TypeOf<HttpRequestException>().And.Property("Message").Contains("500"),
				              () => localStorageClient.Put("id", new Value { Content = "content" }));
			}
		}

		[Test]
		public void GetAllData_Allways_ShouldReturnAllData()
		{
			const string id = "id";
			var value = new Value { Content = "content" };

            using (SimpleStorageService.Start(configuration)) {
				localStorageClient.Put(id, value);
				var actual = localStorageClient.GetAllData();
				Assert.That(actual.ToArray(), Has.Length.EqualTo(1));
			}
		}

		[Test]
		public void GetAllData_StopedInstance_ShouldReturnInternalServerError()
		{
            using (SimpleStorageService.Start(configuration)) {
				serviceClient.Stop();

				Assert.Throws(Is.TypeOf<HttpRequestException>().And.Property("Message").Contains("500"),
				              () => localStorageClient.GetAllData());
			}
		}
	}
}