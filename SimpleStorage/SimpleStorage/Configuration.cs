using System.Linq;
using System.Net;
using SimpleStorage.Infrastructure;

namespace SimpleStorage
{
    public class Configuration : IConfiguration
    {
        public Configuration(ITopology topology)
        {
            IsMaster = !topology.Replicas.Any();
            if (!IsMaster)
                MasterEndpoint = topology.Replicas.First();
        }

        public int ShardNumber { get; set; }
        public bool IsMaster { get; private set; }
        public IPEndPoint MasterEndpoint { get; private set; }
    }
}