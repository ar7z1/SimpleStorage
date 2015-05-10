using System;
using System.Linq;
using System.Threading;
using SimpleStorage.Infrastructure;
using SimpleStorage.IoC;
using StructureMap;
using System.Configuration;
using SimpleStorage.Configuration;

namespace SimpleStorage
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var container = new Container(new SimpleStorageRegistry());
			var config = (SimpleStorageConfigurationSection)ConfigurationManager.GetSection("simpleStorage");
			container.Configure(c => c.For<IServerConfiguration>().Use(config));
			container.Configure(c => c.For<IShardsConfiguration>().Use(config));

			using (SimpleStorageService.Start(string.Format("http://+:{0}/", config.Port), container)) {
				while (true) {
					Console.ReadLine();
				}
			}
		}
	}
}