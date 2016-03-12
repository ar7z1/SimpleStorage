using System.Net;
using System.Net.Http;
using Domain;

namespace Client
{
    public class SimpleStorageConfigurationClient
    {
        private readonly IPEndPoint endpoint;

        public SimpleStorageConfigurationClient(IPEndPoint endpoint)
        {
            this.endpoint = endpoint;
        }

        public void AddShardNode(IPEndPoint shardEndpoint)
        {
            var postUri = string.Format("http://{0}/api/configuration/addShard?endpoint={1}", endpoint, shardEndpoint);
            using (var client = new HttpClient())
            using (var response = client.PostAsync(postUri, null).Result)
                response.EnsureSuccessStatusCode();
        }

        public SimpleStorageConfiguration GetConfiguration()
        {
            var requestUri = string.Format("http://{0}/api/configuration", endpoint);
            using (var client = new HttpClient())
            using (var response = client.GetAsync(requestUri).Result)
            {
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsAsync<SimpleStorageConfiguration>().Result;
            }
        }
    }
}