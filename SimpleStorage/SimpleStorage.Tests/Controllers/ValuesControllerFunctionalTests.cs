using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Client;
using Domain;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using Ploeh.AutoFixture;
using SimpleStorage.Infrastructure;
using SimpleStorage.IoC;

namespace SimpleStorage.Tests.Controllers
{
    [TestFixture]
    public class ValuesControllerFunctionalTests
    {
        [SetUp]
        public void SetUp()
        {
            fixture = new Fixture();
            client = new SimpleStorageClient(endpoint);
            var container = IoCFactory.GetContainer();
            container.Configure(c => c.For<IStateRepository>().Use(new StateRepository()));
            var operationLog = new OperationLog();
            container.Configure(c => c.For<IOperationLog>().Use(operationLog));
            container.Configure(c => c.For<IStorage>().Use(new Storage(operationLog)));
        }

        private const int port = 15000;
        private readonly string endpoint = string.Format("http://127.0.0.1:{0}/", port);
        private Fixture fixture;
        private SimpleStorageClient client;

        [Test]
        public void Delete_UnknownId_ShouldReturnNotFound()
        {
            using (WebApp.Start<Startup>(string.Format("http://+:{0}/", port)))
                Assert.Throws<HttpRequestException>(() => client.Delete(fixture.Create<string>()));
        }

        [Test]
        public void GetAll_NonEmptyStorage_ShouldReturnAll()
        {
            var id = fixture.Create<string>();
            var value = fixture.Create<Value>();
            using (WebApp.Start<Startup>(string.Format("http://+:{0}/", port)))
            {
                client.Put(id, value);

                IEnumerable<ValueWithId> actual = client.GetAll().ToArray();

                Assert.That(actual.ToArray(),
                    Has.Some.Matches<ValueWithId>(v => v.Id == id && v.Value.Content == value.Content));
            }
        }

        [Test]
        public void Get_KnownId_ShouldReturnValue()
        {
            var id = fixture.Create<string>();
            var value = fixture.Create<Value>();

            using (WebApp.Start<Startup>(string.Format("http://+:{0}/", port)))
            {
                client.Put(id, value);
                Value actual = client.Get(id);
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
                        HttpResponseMessage response =
                            httpClient.PostAsync(endpoint + "api/admin/stop", new ByteArrayContent(new byte[0])).Result)
                        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

                    using (HttpResponseMessage response = httpClient.GetAsync(endpoint + "api/values").Result)
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
                    using (HttpResponseMessage response = httpClient.PostAsync(endpoint + "api/admin/stop", new ByteArrayContent(new byte[0])).Result)
                        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

                    using (HttpResponseMessage response = httpClient.PostAsync(endpoint + "api/admin/start", new ByteArrayContent(new byte[0])).Result)
                        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

                    using (HttpResponseMessage response = httpClient.GetAsync(endpoint + "api/values").Result)
                        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                }
            }
        }

        [Test]
        public void Get_UnknownId_ShouldReturnNotFound()
        {
            string requestUri = endpoint + "api/values/" + fixture.Create<string>();

            using (WebApp.Start<Startup>(string.Format("http://+:{0}/", port)))
            using (var client = new HttpClient())
            using (HttpResponseMessage response = client.GetAsync(requestUri).Result)
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}