using System;
using Domain;

namespace SimpleStorage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var port = 0;
            if (args == null || args.Length != 1 || !int.TryParse(args[0], out port))
            {
                Console.Error.WriteLine("Usage: SimpleStorage <port>");
                Environment.Exit(-1);
            }

            using (SimpleStorageService.Start(new SimpleStorageConfiguration(port)))
            {
                while (true)
                {
                    Console.ReadLine();
                }
            }
        }
    }
}