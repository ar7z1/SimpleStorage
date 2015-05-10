using System.Net.Http;
using Domain;
using System.Collections.Generic;

namespace Client
{
	public interface IServiceClient
	{
		void Start();

		void Stop();
	}
}