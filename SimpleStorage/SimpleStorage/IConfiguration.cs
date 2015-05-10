using System.Net;

namespace SimpleStorage
{
    public interface IConfiguration
    {
        int CurrentNodePort { get; }
        bool IsMaster { get; }
        IPEndPoint MasterEndpoint { get; }
    }

}