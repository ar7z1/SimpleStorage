using System.Net;
using System.Collections.Generic;

namespace SimpleStorage.Configuration
{
	public interface IServerConfiguration
	{
		int Port { get; }
	}
}