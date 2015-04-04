using System.Net;

namespace SimpleStorage
{
    public interface IConfiguration
    {
        int ShardNumber { get; }
        bool IsMaster { get; }
        IPEndPoint MasterEndpoint { get; }
    }
}