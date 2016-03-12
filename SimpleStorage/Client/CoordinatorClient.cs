using System.Net;
using System.Net.Http;

namespace Client
{
    public class CoordinatorClient
    {
        private readonly IPEndPoint endpoint;

        public CoordinatorClient(IPEndPoint endpoint)
        {
            this.endpoint = endpoint;
        }

        public IPEndPoint[] Get(string id)
        {
            var requestUri = string.Format("http://{0}/api/coordinate/{1}", endpoint, id);
            using (var client = new HttpClient())
            using (var response = client.GetAsync(requestUri).Result)
            {
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsAsync<IPEndPoint[]>().Result;
            }
        }
    }
}