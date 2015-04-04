using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using Client;
using Domain;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace SimpleStorage.Tests.ReplicationAndConsistency
{
    public class Task1Tests : FuctionalTestBase
    {
        private readonly string masterEndpoint = "http://127.0.0.1:16000/";
        private readonly string slave1Endpoint = "http://127.0.0.1:16001/";
        private readonly string slave2Endpoint = "http://127.0.0.1:16002/";
        private SimpleStorageClient masterClient;
        private SimpleStorageClient slave1Client;
        private SimpleStorageClient slave2Client;

        public override void SetUp()
        {
            base.SetUp();
            masterClient = new SimpleStorageClient(masterEndpoint);
            slave1Client = new SimpleStorageClient(slave1Endpoint);
            slave2Client = new SimpleStorageClient(slave2Endpoint);
        }

        [Test]
        public void Put_OnSlaves_ShouldThrow()
        {
            string id = Guid.NewGuid().ToString();
            var value = new Value {Content = "content"};
            Assert.Throws(CheckHttpException(HttpStatusCode.NotImplemented), () => slave1Client.Put(id, value));
            Assert.Throws(CheckHttpException(HttpStatusCode.NotImplemented), () => slave2Client.Put(id, value));
            masterClient.Put(id, value);
        }

        [Test]
        public void Put_OnMaster_ShouldAvailableOnSlaves()
        {
            string id = Guid.NewGuid().ToString();
            var value = new Value { Content = "content" };
            masterClient.Put(id, value);
            Thread.Sleep(2000);
            var value1 = slave1Client.Get(id);
            Assert.That(value1.Content, Is.EqualTo("content"));
            var value2 = slave2Client.Get(id);
            Assert.That(value2.Content, Is.EqualTo("content"));
        }

        private static Constraint CheckHttpException(HttpStatusCode code)
        {
            return Is.TypeOf<HttpRequestException>().And.Property("Response").Property("StatusCode").EqualTo(code);
        }
    }
}