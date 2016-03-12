using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Domain;

namespace Client
{
    public class OperationLogClient
    {
        private readonly IPEndPoint endpoint;

        public OperationLogClient(IPEndPoint endpoint)
        {
            this.endpoint = endpoint;
        }

        public IEnumerable<Operation> Read(int position, int count)
        {
            var requestUri = string.Format("http://{0}/api/operations?position={1}&count={2}", endpoint, position, count);
            using (var client = new HttpClient())
            using (var response = client.GetAsync(requestUri).Result)
            {
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsAsync<IEnumerable<Operation>>().Result;
            }
        }
    }
}