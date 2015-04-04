using System.Net;

namespace SimpleStorage.Infrastructure
{
    public interface IMasterConfiguration
    {
        bool IsMaster { get; }
        IPEndPoint MasterEndpoint { get; }
    }
}