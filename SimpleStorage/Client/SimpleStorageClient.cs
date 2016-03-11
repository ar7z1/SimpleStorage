using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Domain;
using System.Net;

namespace Client
{
	public class SimpleStorageClient : ISimpleStorageClient
	{
		private readonly IPEndPoint[] endpoints;

		public SimpleStorageClient(params IPEndPoint[] endpoints)
		{
			if (endpoints == null || !endpoints.Any())
				throw new ArgumentException("Empty endpoints!", "endpoints");
			this.endpoints = endpoints;
		}

		public void Put(string id, Value value)
		{
            var endpoint = GetEndpoint(id);
			var requestUri = string.Format("http://{0}/api/values/{1}", endpoint, id);
			using (var client = new HttpClient())
			using (var response = client.PutAsJsonAsync(requestUri, value).Result)
				response.EnsureSuccessStatusCode();
		}

		public Value Get(string id)
		{
            var endpoint = GetEndpoint(id);
            var requestUri = string.Format("http://{0}/api/values/{1}", endpoint, id);
			using (var client = new HttpClient())
			using (var response = client.GetAsync(requestUri).Result) {
				response.EnsureSuccessStatusCode();
				return response.Content.ReadAsAsync<Value>().Result;
			}
		}

        private EndPoint GetEndpoint(string id)
        {
            return endpoints.First();
        }
	}
}