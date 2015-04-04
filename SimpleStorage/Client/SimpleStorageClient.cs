using System;
using System.Linq;
using System.Net.Http;
using Domain;

namespace Client
{
    public class SimpleStorageClient : ISimpleStorageClient
    {
        private readonly string[] endpoints;
        private readonly int attempts;

        public SimpleStorageClient(params string[] endpoints): this(5, endpoints)
        {}

        public SimpleStorageClient(int attempts,params string[] endpoints)
        {
            if (endpoints == null || !endpoints.Any())
                throw new ArgumentException("Empty endpoints!", "endpoints");
            this.endpoints = endpoints;
            this.attempts = attempts;
        }

        public void Put(string id, Value value)
        {
            var rnd = new Random();
            for (var i = 0; i < attempts; ++i)
            {
                try
                {
                    var endpoint = endpoints[rnd.Next(endpoints.Count())];
                    var putUri = endpoint + "api/values/" + id;
                    using (var client = new HttpClient())
                    using (var response = client.PutAsJsonAsync(putUri, value).Result)
                        response.EnsureSuccessStatusCode();
                    return;
                } catch {}
            }
            throw new HttpRequestException("All attempts failed");
        }

        public Value Get(string id)
        {
            var rnd = new Random();
            for (var i = 0; i < attempts; ++i)
            {
                try
                {
                    var endpoint = endpoints[rnd.Next(endpoints.Count())];
                    var requestUri = endpoint + "api/values/" + id;
                    using (var client = new HttpClient())
                    using (var response = client.GetAsync(requestUri).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        return response.Content.ReadAsAsync<Value>().Result;
                    }
                }
                catch { }
            }
            throw new HttpRequestException("All attempts failed");
        }
    }
}