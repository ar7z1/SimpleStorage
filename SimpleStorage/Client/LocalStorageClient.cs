using System.Net.Http;
using Domain;
using System.Collections.Generic;

namespace Client
{
	public class LocalStorageClient : ILocalStorageClient
	{
		private readonly string endpoint;

		public LocalStorageClient(string endpoint)
		{
			this.endpoint = endpoint;
		}

		public void Put(string id, Value value)
		{
			var putUri = endpoint + "api/localStorage/" + id;
			using (var client = new HttpClient())
			using (var response = client.PutAsJsonAsync(putUri, value).Result)
				response.EnsureSuccessStatusCode();
		}

		public Value Get(string id)
		{
			var requestUri = endpoint + "api/localStorage/" + id;
			using (var client = new HttpClient())
			using (var response = client.GetAsync(requestUri).Result) {
				response.EnsureSuccessStatusCode();
				return response.Content.ReadAsAsync<Value>().Result;
			}
		}

		public IEnumerable<ValueWithId> GetAllData()
		{
			var requestUri = endpoint + "api/localStorage/getAllData";
			using (var httpClient = new HttpClient())
			using (var response = httpClient.GetAsync(requestUri).Result) {
				response.EnsureSuccessStatusCode();
				return response.Content.ReadAsAsync<IEnumerable<ValueWithId>>().Result;
			}
		}

	}
}