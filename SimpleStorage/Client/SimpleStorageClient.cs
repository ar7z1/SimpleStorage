using System.Collections.Generic;
using System.Net.Http;
using Domain;

namespace Client
{
    public class SimpleStorageClient : ISimpleStorageClient
    {
        private readonly string endpoint;

        public SimpleStorageClient(string endpoint)
        {
            this.endpoint = endpoint;
        }

        public void Put(string id, Value value)
        {
            string putUri = endpoint + "api/values/" + id;
            using (var client = new HttpClient())
            using (HttpResponseMessage response = client.PutAsJsonAsync(putUri, value).Result)
                response.EnsureSuccessStatusCode();
        }

        public IEnumerable<ValueWithId> GetAll()
        {
            string requestUri = endpoint + "api/values/";
            using (var client = new HttpClient())
            using (HttpResponseMessage response = client.GetAsync(requestUri).Result)
            {
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsAsync<IEnumerable<ValueWithId>>().Result;
            }
        }

        public Value Get(string id)
        {
            string requestUri = endpoint + "api/values/" + id;
            using (var client = new HttpClient())
            using (HttpResponseMessage response = client.GetAsync(requestUri).Result)
            {
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsAsync<Value>().Result;
            }
        }

        public void Delete(string id)
        {
            string uri = endpoint + "api/values/" + id;
            using (var client = new HttpClient())
            using (HttpResponseMessage response = client.DeleteAsync(uri).Result)
                response.EnsureSuccessStatusCode();
        }
    }
}