using System.Net;
using System.Web.Http;
using Domain;

namespace Controllers
{
    public class ConfigurationController : ApiController
    {
        private readonly SimpleStorageConfiguration configuration;

        public ConfigurationController(SimpleStorageConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost]
        public void AddShard(string endpoint)
        {
            configuration.AddShard(Parse(endpoint));
        }

        public SimpleStorageConfiguration Get()
        {
            return configuration;
        }

        private static IPEndPoint Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return null;

            var splitted = s.Split(':');
            if (splitted.Length != 2)
                return null;

            IPAddress ip;
            int port;
            if (!IPAddress.TryParse(splitted[0], out ip) || !int.TryParse(splitted[1], out port))
                return null;

            return new IPEndPoint(ip, port);
        }
    }
}