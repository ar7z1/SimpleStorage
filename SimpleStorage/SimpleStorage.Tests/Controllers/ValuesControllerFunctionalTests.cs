using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Client;
using Domain;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using Ploeh.AutoFixture;

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
        public void Get_UnknownId_ShouldReturnNotFound()
        {
            using (WebApp.Start<Startup>(string.Format("http://+:{0}/", port)))
            using (var client = new HttpClient())
            {
                string requestUri = endpoint + "api/values/" + fixture.Create<string>();
                using (HttpResponseMessage response = client.GetAsync(requestUri).Result)
                    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            }
        }
    }
}