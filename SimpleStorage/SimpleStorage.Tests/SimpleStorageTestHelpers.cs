using System;
using SimpleStorage.Infrastructure;
using SimpleStorage.IoC;
using StructureMap;

namespace SimpleStorage.Tests
{
	public class SimpleStorageTestHelpers
	{
		public static IDisposable StartService(int servicePort)
		{
			var container = new Container(new SimpleStorageRegistry());
			var topology = new Topology(new int[0]);
			var configuration = new OldConfiguration(topology) {
				CurrentNodePort = servicePort
			};
			container.Configure(c => c.For<IConfiguration>().Use(configuration));
			return SimpleStorageService.Start(string.Format("http://+:{0}/", servicePort), container);
		}

	}
}