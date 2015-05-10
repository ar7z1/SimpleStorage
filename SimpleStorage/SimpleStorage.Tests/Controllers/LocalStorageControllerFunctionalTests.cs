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
		private IPEndPoint EndPoint = new IPEndPoint(IPAddress.Loopback, port);
		private ServiceClient serviceClient;

		[SetUp]
		public void SetUp()
		{
			localStorageClient = new LocalStorageClient(EndPoint);
			serviceClient = new ServiceClient(EndPoint);
		}

		[Test]
		public void Get_KnownId_ShouldReturnValue()
		{
			const string id = "id";
			var value = new Value { Content = "content" };

			using (SimpleStorageTestHelpers.StartService(port)) {
				localStorageClient.Put(id, value);
				var actual = localStorageClient.Get(id);
				Assert.That(actual.Content, Is.EqualTo(value.Content));
			}
		}

		[Test]
		public void Get_UnknownId_ShouldReturnNotFound()
		{
			using (SimpleStorageTestHelpers.StartService(port)) {
				Assert.Throws(Is.TypeOf<HttpRequestException>().And.Property("Message").Contains("404"),
				              () => localStorageClient.Get("unknownId"));
			}
		}

		[Test]
		public void Get_StoppedInstance_ShouldReturnInternalServerError()
		{
			using (SimpleStorageTestHelpers.StartService(port)) {
				serviceClient.Stop();

				Assert.Throws(Is.TypeOf<HttpRequestException>().And.Property("Message").Contains("500"),
				              () => localStorageClient.Get("unknownId"));
			}
		}

		[Test]
		public void Put_StoppedInstance_ShouldReturnInternalServerError()
		{
			using (SimpleStorageTestHelpers.StartService(port)) {
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

			using (SimpleStorageTestHelpers.StartService(port)) {
				localStorageClient.Put(id, value);
				var actual = localStorageClient.GetAllData();
				Assert.That(actual.ToArray(), Has.Length.EqualTo(1));
			}
		}

		[Test]
		public void GetAllData_StopedInstance_ShouldReturnInternalServerError()
		{
			using (SimpleStorageTestHelpers.StartService(port)) {
				serviceClient.Stop();

				Assert.Throws(Is.TypeOf<HttpRequestException>().And.Property("Message").Contains("500"),
				              () => localStorageClient.GetAllData());
			}
		}
	}
}