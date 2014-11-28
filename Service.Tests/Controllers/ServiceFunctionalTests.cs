using System.Net;
using System.Net.Http;
using NUnit.Framework;

namespace Service.Tests.Controllers
{
    public class ServiceFunctionalTests : TestBase
    {
        private const string endpoint = "http://localhost:15000/";

        [Test]
        public void Get_Always_ShouldReturnOk()
        {
            var client = new HttpClient();
            var response = client.GetAsync(endpoint + "api/values").Result;
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }
}