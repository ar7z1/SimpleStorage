using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Domain;
using SimpleStorage.Infrastructure;

namespace SimpleStorage.Controllers
{
	public class LocalStorageController : ApiController
	{
		private readonly IStateRepository stateRepository;
		private readonly IStorage storage;

		public LocalStorageController(IStateRepository stateRepository, IStorage storage)
		{
			this.stateRepository = stateRepository;
			this.storage = storage;
		}

		[HttpGet]
		public Value Get(string id)
		{
			stateRepository.ThrowIfNotStarted();
			var result = storage.Get(id);
			if (result == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);
			return result;
		}

		[HttpPut]
		public void Put(string id, [FromBody] Value value)
		{
			stateRepository.ThrowIfNotStarted();
			storage.Set(id, value);
		}

		[HttpGet]
		public IEnumerable<ValueWithId> GetAllData()
		{
			stateRepository.ThrowIfNotStarted();
			return storage.GetAll().ToArray();
		}

	}
}