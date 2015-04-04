using System.Linq;
using System.Net;
using System.Net.Http;
using Client;
using Domain;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using SimpleStorage.Infrastructure;
using SimpleStorage.IoC;

namespace SimpleStorage.Tests.Controllers
{
    [TestFixture]
    public class ValuesControllerFunctionalTests
    {
        private const int port = 15000;
        private readonly string endpoint = string.Format("http://127.0.0.1:{0}/", port);
        private SimpleStorageClient client;

        [SetUp]
        public void SetUp()
        {
            client = new SimpleStorageClient(endpoint);
            var container = IoCFactory.GetContainer();
            container.Configure(c => c.For<IStateRepository>().Use(new StateRepository()));
            var operationLog = new OperationLog();
            container.Configure(c => c.For<IOperationLog>().Use(operationLog));
            container.Configure(
                c => c.For<IStorage>().Use(new Storage(operationLog, container.GetInstance<ValueComparer>())));
        }

        [Test]
        public void GetAll_NonEmptyStorage_ShouldReturnAll()
        {
            const string id = "id";
            var value = new Value {Content = "content"};
            using (WebApp.Start<Startup>(string.Format("http://+:{0}/", port)))
            {
                client.Put(id, value);

                var actual = client.GetAll().ToArray();

                Assert.That(actual.ToArray(),
                    Has.Some.Matches<ValueWithId>(v => v.Id == id && v.Value.Content == value.Content));
            }
        }

        [Test]
        public void Get_KnownId_ShouldReturnValue()
        {
            const string id = "id";
            var value = new Value {Content = "content"};

            using (WebApp.Start<Startup>(string.Format("http://+:{0}/", port)))
            {
                client.Put(id, value);
                var actual = client.Get(id);
                Assert.That(actual.Content, Is.EqualTo(value.Content));
            }
        }

        [Test]
        public void Get_StopInstance_ShouldThrow()
        {
            using (WebApp.Start<Startup>(string.Format("http://+:{0}/", port)))
            {
                using (var httpClient = new HttpClient())
                {
                    using (
                        var response =
                            httpClient.PostAsync(endpoint + "api/admin/stop", new ByteArrayContent(new byte[0])).Result)
                        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

                    using (var response = httpClient.GetAsync(endpoint + "api/values").Result)
                        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
                }
            }
        }

        [Test]
        public void Get_StartInstance_ShouldNotThrow()
        {
            using (WebApp.Start<Startup>(string.Format("http://+:{0}/", port)))
            {
                using (var httpClient = new HttpClient())
                {
                    using (
                        var response =
                            httpClient.PostAsync(endpoint + "api/admin/stop", new ByteArrayContent(new byte[0])).Result)
                        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

                    using (
                        var response =
                            httpClient.PostAsync(endpoint + "api/admin/start", new ByteArrayContent(new byte[0])).Result
                        )
                        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

                    using (var response = httpClient.GetAsync(endpoint + "api/values").Result)
                        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                }
            }
        }

        [Test]
        public void Get_UnknownId_ShouldReturnNotFound()
        {
            var requestUri = endpoint + "api/values/unknownId";

            using (WebApp.Start<Startup>(string.Format("http://+:{0}/", port)))
            using (var httpClient = new HttpClient())
            using (var response = httpClient.GetAsync(requestUri).Result)
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}