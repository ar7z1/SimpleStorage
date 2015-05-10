using System.Net;
using System.Collections.Generic;
using System.Configuration;

namespace SimpleStorage.Configuration
{
	public class ShardConfiguration : ConfigurationElement
	{
		private const string NameKeyName = "name";
		private const string IPAddressKeyName = "ipAddress";
		private const string PortKeyName = "port";

		[ConfigurationProperty(NameKeyName, IsKey = true, IsRequired = true)]
		public string Name {
			get {
				return (string)base[NameKeyName];
			}
		}

		[ConfigurationProperty(IPAddressKeyName, IsRequired = true)]
		private string IPAddressString {
			get {
				return (string)base[IPAddressKeyName];
			}
		}

		[ConfigurationProperty(PortKeyName, IsRequired = true)]
		private int Port {
			get {
				return (int)base[PortKeyName];
			}
		}

		public IPEndPoint IPEndpoint {
			get {
				return new IPEndPoint(IPAddress.Parse(IPAddressString), Port);
			}
		}
	}

}