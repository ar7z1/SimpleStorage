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
		private IPEndPoint EndPoint = new IPEndPoint(IPAddress.Loopback, port);
		private ServiceClient serviceClient;

		[SetUp]
		public void SetUp()
		{
			simpleStorageClient = new SimpleStorageClient(EndPoint);
			serviceClient = new ServiceClient(EndPoint);
		}

		[Test]
		public void Get_KnownId_ShouldReturnValue()
		{
			const string id = "id";
			var value = new Value { Content = "content" };

			using (SimpleStorageTestHelpers.StartService(port)) {
				simpleStorageClient.Put(id, value);
				var actual = simpleStorageClient.Get(id);
				Assert.That(actual.Content, Is.EqualTo(value.Content));
			}
		}

		[Test]
		public void Get_StoppedInstance_ShouldThrow()
		{
			using (SimpleStorageTestHelpers.StartService(port)) {
				serviceClient.Stop();

				Assert.Throws(Is.TypeOf<HttpRequestException>().And.Property("Message").Contains("500"),
				              () => simpleStorageClient.Get("unknownId"));
			}
		}

		[Test]
		public void Get_StartInstance_ShouldNotThrow()
		{
			using (SimpleStorageTestHelpers.StartService(port)) {
				simpleStorageClient.Put("id", new Value());

				serviceClient.Stop();
				serviceClient.Start();
				Assert.DoesNotThrow(() => simpleStorageClient.Get("id"));
			}
		}

		[Test]
		public void Get_UnknownId_ShouldReturnNotFound()
		{
			using (SimpleStorageTestHelpers.StartService(port)) {
				Assert.Throws(Is.TypeOf<HttpRequestException>().And.Property("Message").Contains("404"),
				              () => simpleStorageClient.Get("unknownId"));
			}
		}
	}
}