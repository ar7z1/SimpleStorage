using System;
using System.Linq;
using System.Net.Http;
using Domain;

namespace Client
{
    public class SimpleStorageClient : ISimpleStorageClient
    {
        private readonly string[] endpoints;

        public SimpleStorageClient(params string[] endpoints)
        {
            if (endpoints == null || !endpoints.Any())
                throw new ArgumentException("Empty endpoints!", "endpoints");
            this.endpoints = endpoints.ToArray();
        }

        public void Put(string id, Value value)
        {
            var putUri = endpoints.First() + "api/values/" + id;
            using (var client = new HttpClient())
            using (var response = client.PutAsJsonAsync(putUri, value).Result)
                response.EnsureSuccessStatusCode();
        }

        public Value Get(string id)
        {
            var random = new Random();
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    var requestUri = endpoints[random.Next(endpoints.Count() - 1)] + "api/values/" + id;
                    using (var client = new HttpClient())
                    using (var response = client.GetAsync(requestUri).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        return response.Content.ReadAsAsync<Value>().Result;
                    }
                } catch {}
            }
            throw new HttpRequestException("All attempts failed");
        }
    }
}