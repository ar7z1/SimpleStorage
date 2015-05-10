using System;
using SimpleStorage.Infrastructure;
using SimpleStorage.IoC;

namespace SimpleStorage.Tests
{
	public class SimpleStorageTestHelpers
	{
		public static IDisposable StartService(int servicePort)
		{
			var container = IoCFactory.NewContainer();
			var topology = new Topology(new int[0]);
			var configuration = new Configuration(topology) {
				CurrentNodePort = servicePort
			};
			container.Configure(c => c.For<IConfiguration>().Use(configuration));
			return SimpleStorageService.Start(string.Format("http://+:{0}/", servicePort), container);
		}

	}
}