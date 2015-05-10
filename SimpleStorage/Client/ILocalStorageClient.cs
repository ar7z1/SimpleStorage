using System.Net.Http;
using Domain;
using System.Collections.Generic;

namespace Client
{
	public interface ILocalStorageClient : ISimpleStorageClient
	{
		IEnumerable<ValueWithId> GetAllData();
	}
}