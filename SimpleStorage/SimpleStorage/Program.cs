using System;
using System.Linq;
using CommandLine;
using CommandLine.Text;
using Microsoft.Owin.Hosting;
using SimpleStorage.Infrastructure;
using SimpleStorage.IoC;

namespace SimpleStorage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
                var container = IoCFactory.GetContainer();

                container.Configure(c => c.For<ITopology>().Use(new Topology(options.ReplicasPorts)).Singleton());

                var currentNodeShardNumber = options.Port%(options.ShardsPorts.Length + 1);
                var configuration = new Configuration {ShardNumber = currentNodeShardNumber};
                container.Configure(c => c.For<IConfiguration>().Use(configuration));

                using (WebApp.Start<Startup>(string.Format("http://+:{0}/", options.Port)))
                {
                    Console.WriteLine("Server running on port {0}", options.Port);

                    if (options.ReplicasPorts.Any())
                        Console.WriteLine("Replicas running on ports {0}", string.Join(", ", options.ReplicasPorts));

                    if (options.ShardsPorts.Any())
                        Console.WriteLine("Shards running on ports {0}", string.Join(", ", options.ReplicasPorts));

                    Console.ReadLine();
                }
            }
        }

        private class Options
        {
            [Option('p', Required = true, HelpText = "Port.")]
            public int Port { get; set; }

            [Option("rp", Required = false, HelpText = "Replicas ports.")]
            public string ReplicasPortsString { get; set; }

            [Option("sp", Required = false, HelpText = "Shards ports.")]
            public string ShardsPortsString { get; set; }

            public int[] ReplicasPorts
            {
                get
                {
                    if (string.IsNullOrEmpty(ReplicasPortsString))
                        return new int[0];
                    return ReplicasPortsString.Split(',').Select(int.Parse).ToArray();
                }
            }

            public int[] ShardsPorts
            {
                get
                {
                    if (string.IsNullOrEmpty(ShardsPortsString))
                        return new int[0];
                    return ShardsPortsString.Split(',').Select(int.Parse).ToArray();
                }
            }

            [HelpOption]
            public string GetUsage()
            {
                var result = HelpText.AutoBuild(this,
                    current => HelpText.DefaultParsingErrorsHandler(this, current));
                return result;
            }
        }
    }
}