using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Domain;

namespace Client
{
    public class LocalStorageClient
    {
        private readonly IPEndPoint endpoint;

        public LocalStorageClient(IPEndPoint endpoint)
        {
            this.endpoint = endpoint;
        }

        public void Put(string id, Value value)
        {
            var putUri = string.Format("http://{0}/api/localStorage/{1}", endpoint, id);
            using (var client = new HttpClient())
            using (var response = client.PutAsJsonAsync(putUri, value).Result)
                response.EnsureSuccessStatusCode();
        }

        public Value Get(string id)
        {
            var requestUri = string.Format("http://{0}/api/localStorage/{1}", endpoint, id);
            using (var client = new HttpClient())
            using (var response = client.GetAsync(requestUri).Result)
            {
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsAsync<Value>().Result;
            }
        }

        public IEnumerable<ValueWithId> GetAllData()
        {
            var requestUri = string.Format("http://{0}/api/localStorage", endpoint);
            using (var httpClient = new HttpClient())
            using (var response = httpClient.GetAsync(requestUri).Result)
            {
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsAsync<IEnumerable<ValueWithId>>().Result;
            }
        }
    }
}