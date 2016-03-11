using System.Web.Http;
using Domain;
using System.Linq;
using System.Net;
using System;

namespace Coordinator.Controllers
{
    public class CoordinatorController : ApiController
    {
        private SimpleStorageConfiguration configuration;

        public CoordinatorController(SimpleStorageConfiguration configuration)
        {
            if (configuration.Topology == null || !configuration.Topology.Any() || configuration.Topology.Any(s => s == null || !s.Any()))
                throw new ArgumentException("Bad configuration!", "configuration");
            this.configuration = configuration;
        }

        public IPEndPoint[] Get(string id)
        {
            return configuration.Topology.First();
        }
    }
}