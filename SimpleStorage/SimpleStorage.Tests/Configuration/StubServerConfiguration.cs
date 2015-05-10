using System;
using SimpleStorage.Configuration;

namespace Configuration
{
	public class StubServerConfiguration : IServerConfiguration
	{
		public int Port { get; set; }
	}
}

