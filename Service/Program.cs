using System;
using CommandLine;
using CommandLine.Text;
using Microsoft.Owin.Hosting;

namespace Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
                string url = string.Format("http://+:{0}/", options.Port);
                using (WebApp.Start<Startup>(url))
                {
                    Console.WriteLine("Server running on {0}", url);
                    foreach (string replicaUrl in options.ReplicaUrls)
                        Console.WriteLine("Replica: {0}", replicaUrl);
                    Console.ReadLine();
                }
            }
        }

        private class Options
        {
            [Option('p', Required = true, HelpText = "Port.")]
            public int Port { get; set; }

            [OptionArray('r', Required = true, HelpText = "Replica urls.")]
            public string[] ReplicaUrls { get; set; }

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