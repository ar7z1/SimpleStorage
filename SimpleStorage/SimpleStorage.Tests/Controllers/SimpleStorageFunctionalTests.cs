using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using Ploeh.AutoFixture;
using SimpleStorage.Infrastructure;

namespace SimpleStorage.Tests.Controllers
{
    [TestFixture]
    public class SimpleStorageFunctionalTests
    {
        private const int port = 15000;
        private readonly string endpoint = string.Format("http://127.0.0.1:{0}/", port);
        private Fixture fixture;

        [SetUp]
        public void SetUp()
        {
            fixture = new Fixture();
        }

        private void Put(string id, Value value)
        {
            var putUri = endpoint + "api/values/" + id;
            using (var client = new HttpClient())
            using (var response = client.PutAsJsonAsync(putUri, value).Result)
                response.EnsureSuccessStatusCode();
        }

        private IEnumerable<ValueWithId> GetAll()
        {
            var requestUri = endpoint + "api/values/";
            using (var client = new HttpClient())
            using (var response = client.GetAsync(requestUri).Result)
            {
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsAsync<IEnumerable<ValueWithId>>().Result;
            }
        }

        private Value Get(string id)
        {
            var requestUri = endpoint + "api/values/" + id;
            using (var client = new HttpClient())
            using (var response = client.GetAsync(requestUri).Result)
            {
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsAsync<Value>().Result;
            }
        }

        [Test]
        public void Delete_UnknownId_ShouldReturnNotFound()
        {
            var requestUri = endpoint + "api/values/" + fixture.Create<string>();
            using (WebApp.Start<Startup>(string.Format("http://+:{0}/", port)))
            using (var client = new HttpClient())
            using (var response = client.DeleteAsync(requestUri).Result)
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public void GetAll_EmptyStorage_ShouldReturnEmpty()
        {
            using (WebApp.Start<Startup>(string.Format("http://+:{0}/", port)))
            {
                var actual = GetAll();
                Assert.That(actual, Is.Empty);
            }
        }

        [Test]
        public void GetAll_NonEmptyStorage_ShouldReturnAll()
        {
            var id = fixture.Create<string>();
            var value = fixture.Create<Value>();
            using (WebApp.Start<Startup>(string.Format("http://+:{0}/", port)))
            {
                Put(id, value);

                IEnumerable<ValueWithId> actual = GetAll().ToArray();

                Assert.That(actual.ToArray(), Has.Some.Matches<ValueWithId>(v => v.Id == id && v.Value.Content == value.Content));
            }
        }

        [Test]
        public void Get_KnownId_ShouldReturnValue()
        {
            var id = fixture.Create<string>();
            var value = fixture.Create<Value>();

            using (WebApp.Start<Startup>(string.Format("http://+:{0}/", port)))
            {
                Put(id, value);
                var actual = Get(id);
                Assert.That(actual.Content, Is.EqualTo(value.Content));
            }
        }

        [Test]
        public void Get_UnknownId_ShouldReturnNotFound()
        {
            using (WebApp.Start<Startup>(string.Format("http://+:{0}/", port)))
            using (var client = new HttpClient())
            {
                var requestUri = endpoint + "api/values/" + fixture.Create<string>();
                using (var response = client.GetAsync(requestUri).Result)
                    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            }
        }
    }
}