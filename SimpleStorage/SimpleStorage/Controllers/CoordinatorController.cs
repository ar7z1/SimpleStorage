using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using Domain;

namespace Coordinator.Controllers
{
    public class CoordinatorController : ApiController
    {
        private readonly SimpleStorageConfiguration configuration;

        public CoordinatorController(SimpleStorageConfiguration configuration)
        {
            if (configuration.Topology == null || !configuration.Topology.Any() ||
                configuration.Topology.Any(s => s == null || !s.Any()))
                throw new ArgumentException("Bad configuration!", "configuration");
            this.configuration = configuration;
        }

        public IPEndPoint[] Get(string id)
        {
            return configuration.Topology.First();
        }
    }
}