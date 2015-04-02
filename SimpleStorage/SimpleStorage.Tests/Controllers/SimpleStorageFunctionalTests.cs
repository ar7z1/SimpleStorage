using System.Net;
using System.Net.Http;
using Microsoft.Owin.Hosting;
using NUnit.Framework;

namespace SimpleStorage.Tests.Controllers
{
    [TestFixture]
    public class SimpleStorageFunctionalTests
    {
        private const int port = 15000;
        private readonly string endpoint = string.Format("http://127.0.0.1:{0}/", port);

        [Test]
        public void Get_Always_ShouldReturnOk()
        {
            using (WebApp.Start<Startup>(string.Format("http://+:{0}/", port)))
            {
                var client = new HttpClient();
                var response = client.GetAsync(endpoint + "api/values").Result;
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            }
        }
    }
}