using System;
using Microsoft.Owin.Hosting;

namespace Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string url = "http://localhost:15000";
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("Server running on {0}", url);
                Console.ReadLine();
            }
        }
    }
}