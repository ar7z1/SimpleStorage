using System.Linq;
using System.Net;

namespace SimpleStorage.Infrastructure
{
    public class MasterConfiguration : IMasterConfiguration
    {
        public MasterConfiguration(ITopology topology)
        {
            IsMaster = !topology.Replicas.Any();
            if (!IsMaster)
                MasterEndpoint = topology.Replicas.First();
        }

        public bool IsMaster { get; private set; }
        public IPEndPoint MasterEndpoint { get; private set; }
    }
}