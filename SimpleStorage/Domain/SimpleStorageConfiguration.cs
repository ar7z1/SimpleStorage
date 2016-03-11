using System.Net;
using System.Collections.Generic;

namespace Domain
{
    public class SimpleStorageConfiguration
    {
        List<IPEndPoint> shards;

        List<IPEndPoint> replicas;

        public SimpleStorageConfiguration(int port)
        {
            Port = port;
            shards = new List<IPEndPoint>();
            replicas = new List<IPEndPoint>();
        }

        public int Port { get; }

        public IEnumerable<IPEndPoint> Shards { get { return shards; } }

        public void AddShard(IPEndPoint endpoint)
        {
            shards.Add(endpoint);
        }

        public IEnumerable<IPEndPoint> Replicas { get { return replicas; } }

        public void AddReplica(IPEndPoint endpoint)
        {
            replicas.Add(endpoint);
        }

        public IPEndPoint Master { get; set; }
    }
}