using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Client;
using Domain;
using NUnit.Framework;

namespace SimpleStorage.Tests
{
    [TestFixture]
    [Ignore]
    public class ShardingTests
    {
        private const int port1 = 15000;
        private const int port2 = 15001;
        private const int port3 = 15002;
        private readonly string[] endpoints = {endpoint1, endpoint2, endpoint3};
        private SimpleStorageClient simpleStorageClient;

        [SetUp]
        public void SetUp()
        {
            simpleStorageClient = new SimpleStorageClient(endpoints);
        }

        [Test]
        public void Sharding_EachShard_ShouldNotContainAllData()
        {
            using (SimpleStorageTestHelpers.StartService(port1))
            using (SimpleStorageTestHelpers.StartService(port2))
            using (SimpleStorageTestHelpers.StartService(port3))
            {
                for (var i = 0; i < 100; i++)
                    simpleStorageClient.Put(Guid.NewGuid().ToString(), new Value {Content = "content"});

                Assert.That(GetAll(endpoint1).ToArray(), Has.Length.LessThan(100));
                Assert.That(GetAll(endpoint2).ToArray(), Has.Length.LessThan(100));
                Assert.That(GetAll(endpoint3).ToArray(), Has.Length.LessThan(100));
            }
        }

        [Test]
        public void Sharding_AllShards_ShouldContainSomeData()
        {
            using (SimpleStorageTestHelpers.StartService(port1))
            using (SimpleStorageTestHelpers.StartService(port2))
            using (SimpleStorageTestHelpers.StartService(port3))
            {
                for (var i = 0; i < 100; i++)
                    simpleStorageClient.Put(Guid.NewGuid().ToString(), new Value {Content = "content"});

                Assert.That(GetAll(endpoint1).ToArray(), Has.Length.GreaterThan(0));
                Assert.That(GetAll(endpoint2).ToArray(), Has.Length.GreaterThan(0));
                Assert.That(GetAll(endpoint3).ToArray(), Has.Length.GreaterThan(0));
            }
        }

        [Test]
        public void Sharding_Always_ShouldSaveAllData()
        {
            using (SimpleStorageTestHelpers.StartService(port1))
            using (SimpleStorageTestHelpers.StartService(port2))
            using (SimpleStorageTestHelpers.StartService(port3))
            {
                var items = new List<KeyValuePair<string, Value>>();
                for (var i = 0; i < 100; i++)
                {
                    var id = Guid.NewGuid().ToString();
                    var value = new Value {Content = "content"};
                    items.Add(new KeyValuePair<string, Value>(id, value));
                    simpleStorageClient.Put(id, value);
                }

                foreach (var item in items)
                {
                    var actual = simpleStorageClient.Get(item.Key);
                    Assert.That(actual.Content, Is.EqualTo(item.Value.Content));
                    Assert.That(actual.IsDeleted, Is.EqualTo(item.Value.IsDeleted));
                    Assert.That(actual.Revision, Is.EqualTo(item.Value.Revision));
                }
            }
        }

        private IEnumerable<ValueWithId> GetAll(string endpoint)
        {
            var requestUri = endpoint + "api/admin/getAllLocalData";
            using (var httpClient = new HttpClient())
            using (var response = httpClient.GetAsync(requestUri).Result)
            {
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsAsync<IEnumerable<ValueWithId>>().Result;
            }
        }

        private static readonly string endpoint1 = string.Format("http://127.0.0.1:{0}/", port1);
        private static readonly string endpoint2 = string.Format("http://127.0.0.1:{0}/", port2);
        private static readonly string endpoint3 = string.Format("http://127.0.0.1:{0}/", port3);
    }
}