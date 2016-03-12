using System.Net;
using System.Net.Http;

namespace Client
{
    public class ServiceClient
    {
        private readonly IPEndPoint endpoint;

        public ServiceClient(IPEndPoint endpoint)
        {
            this.endpoint = endpoint;
        }

        public void Stop()
        {
            var uri = string.Format("http://{0}/api/service/stop", endpoint);
            using (var client = new HttpClient())
            using (var response = client.PostAsync(uri, new ByteArrayContent(new byte[0])).Result)
                response.EnsureSuccessStatusCode();
        }

        public void Start()
        {
            var uri = string.Format("http://{0}/api/service/start", endpoint);
            using (var client = new HttpClient())
            using (var response = client.PostAsync(uri, new ByteArrayContent(new byte[0])).Result)
                response.EnsureSuccessStatusCode();
        }
    }
}