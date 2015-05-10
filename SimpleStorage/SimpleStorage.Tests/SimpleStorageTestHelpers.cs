using System;
using SimpleStorage.Infrastructure;
using SimpleStorage.IoC;
using StructureMap;
using Configuration;
using System.Net;
using SimpleStorage.Configuration;

namespace SimpleStorage.Tests
{
	public class SimpleStorageTestHelpers
	{
		public static IDisposable StartService(int servicePort, IShardsConfiguration shardsConfiguration = null)
		{
			var container = new Container(new SimpleStorageRegistry());
			var serverConfig = new StubServerConfiguration(){ Port = servicePort };
			var shardsConfig = shardsConfiguration ??
			                   new StubShardsConfiguration() { Shards = new[] { new IPEndPoint(IPAddress.Loopback, servicePort) } };
			container.Configure(c => c.For<IServerConfiguration>().Use(serverConfig));
			container.Configure(c => c.For<IShardsConfiguration>().Use(shardsConfig));
			return SimpleStorageService.Start(string.Format("http://+:{0}/", servicePort), container);
		}
	}
}