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
                IoCFactory.GetContainer()
                    .Configure(c => c.For<ITopology>().Use(new Topology(options.Ports)).Singleton());
                using (WebApp.Start<Startup>(string.Format("http://+:{0}/", options.Port)))
                {
                    Console.WriteLine("Server running on port {0}", options.Port);
                    Console.WriteLine("Replicas running on ports {0}", string.Join(", ", options.Ports));
                    Console.ReadLine();
                }
            }
        }

        private class Options
        {
            [Option('p', Required = true, HelpText = "Port.")]
            public int Port { get; set; }

            [Option("rp", Required = false, HelpText = "Ports.")]
            public string PortsString { get; set; }

            public int[] Ports
            {
                get
                {
                    if (string.IsNullOrEmpty(PortsString))
                        return new int[0];
                    return PortsString.Split(',').Select(int.Parse).ToArray();
                }
            }

            [HelpOption]
            public string GetUsage()
            {
                HelpText result = HelpText.AutoBuild(this,
                    current => HelpText.DefaultParsingErrorsHandler(this, current));
                return result;
            }
        }
    }
}