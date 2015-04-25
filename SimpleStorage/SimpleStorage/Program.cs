﻿using System;
using System.Linq;
using System.Threading;
using CommandLine;
using CommandLine.Text;
using SimpleStorage.Infrastructure;
using SimpleStorage.Infrastructure.Replication;
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
                var container = IoCFactory.NewContainer();

                var topology = new Topology(options.ReplicasPorts);
                container.Configure(c => c.For<ITopology>().Use(topology).Singleton());

                var configuration = new Configuration(topology)
                {
                    CurrentNodePort = options.Port,
                    OtherShardsPorts = options.ShardsPorts
                };
                container.Configure(c => c.For<IConfiguration>().Use(configuration));
                using (SimpleStorageService.Start(string.Format("http://+:{0}/", options.Port), container))
                {
                    Console.WriteLine("Server running on port {0}", options.Port);
                    var cts = new CancellationTokenSource();
                    CancellationToken cancellationToken = cts.Token;
                    var synchronizationTask = container.GetInstance<IOperationLogSynchronizer>().Synchronize(cancellationToken);
                    Console.CancelKeyPress += delegate
                    {
                        cts.Cancel();
                        synchronizationTask.Wait(cancellationToken);
                    };
                    if (options.ReplicasPorts.Any())
                        Console.WriteLine("Replicas running on ports {0}", string.Join(", ", options.ReplicasPorts));

                    if (options.ShardsPorts.Any())
                        Console.WriteLine("Shards running on ports {0}", string.Join(", ", options.ShardsPorts));
                    while (true) {
                        Console.ReadLine();
                    }
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