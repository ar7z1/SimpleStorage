using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Domain
{
    public class SimpleStorageConfiguration
    {
        private List<IPEndPoint> shards;

        public SimpleStorageConfiguration(int port, IPEndPoint[][] topology = null)
        {
            Port = port;

            if (topology != null)
            {
                if (topology.Any(s => s == null || !s.Any()))
                    throw new ArgumentException("Bad topology!", "topology");
                Topology = topology;
            }
        }

        public int Port { get; private set; }

        public IPEndPoint[][] Topology { get; private set; }

        public IPEndPoint CurrentNodeEndpoint
        {
            get { return new IPEndPoint(IPAddress.Loopback, Port); }
        }

        public IPEndPoint Master { get; set; }

        //todo kill
        public IEnumerable<IPEndPoint> Shards
        {
            get { return shards; }
        }

        public void AddShard(IPEndPoint endpoint)
        {
            shards.Add(endpoint);
        }
    }
}