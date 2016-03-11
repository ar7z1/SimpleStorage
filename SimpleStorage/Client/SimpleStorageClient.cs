using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Domain;
using System.Net;

namespace Client
{
	public class SimpleStorageClient
	{
        private readonly IPEndPoint[][] topology;

        public SimpleStorageClient(IPEndPoint[][] topology)
		{
            if (topology == null || !topology.Any())
				throw new ArgumentException("Empty topology!", "topology");
            if (topology.Any(s => s == null || !s.Any()))
                throw new ArgumentException("Bad topology!", "topology");
            this.topology = topology;
		}

        public SimpleStorageClient(IPEndPoint endpoint)
        {
            if (endpoint == null)
                throw new ArgumentException("Null endpoint!", "endpoint");

            topology = new []{ new[]{ endpoint } };
        }

		public void Put(string id, Value value)
		{
            var replicas = GetShardEndpoint(id);
            var endpoint = replicas.First();
            var requestUri = string.Format("http://{0}/api/values/{1}", endpoint, id);
			using (var client = new HttpClient())
			using (var response = client.PutAsJsonAsync(requestUri, value).Result)
				response.EnsureSuccessStatusCode();
		}

		public Value Get(string id)
		{
            var replicas = GetShardEndpoint(id);
            var endpoint = replicas.First();
            var requestUri = string.Format("http://{0}/api/values/{1}", endpoint, id);
			using (var client = new HttpClient())
			using (var response = client.GetAsync(requestUri).Result) {
				response.EnsureSuccessStatusCode();
				return response.Content.ReadAsAsync<Value>().Result;
			}
		}

        private IEnumerable<EndPoint> GetShardEndpoint(string id)
        {
            return topology.First();
        }
	}
}