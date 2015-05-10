using System.Net;
using System.Web.Http;
using Domain;
using SimpleStorage.Infrastructure;
using SimpleStorage.Configuration;

namespace SimpleStorage.Controllers
{
	public class ValuesController : ApiController
	{
		private readonly IStateRepository stateRepository;
		private readonly IStorage storage;
		private readonly IServerConfiguration serverConfiguration;
		private readonly IShardsConfiguration shardsConfiguration;

		public ValuesController(IStorage storage,
		                        IStateRepository stateRepository,
		                        IServerConfiguration serverConfiguration,
		                        IShardsConfiguration shardsConfiguration)
		{
			this.shardsConfiguration = shardsConfiguration;
			this.serverConfiguration = serverConfiguration;
			this.storage = storage;
			this.stateRepository = stateRepository;
		}

		// GET api/values/5
		public Value Get(string id)
		{
			stateRepository.ThrowIfNotStarted();
			var result = storage.Get(id);
			if (result == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);
			return result;
		}

		// PUT api/values/5
		public void Put(string id, [FromBody] Value value)
		{
			stateRepository.ThrowIfNotStarted();
			storage.Set(id, value);
		}
	}
}