using System.Linq;
using System.Net;
using System.Web.Http;
using Domain;
using SimpleStorage.Infrastructure;

namespace SimpleStorage.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly SimpleStorageConfiguration configuration;
        private readonly IStateRepository stateRepository;
        private readonly IStorage storage;

        public ValuesController(IStorage storage,
            IStateRepository stateRepository,
            SimpleStorageConfiguration configuration)
        {
            this.configuration = configuration;
            this.storage = storage;
            this.stateRepository = stateRepository;
        }

        public Value Get(string id)
        {
            stateRepository.ThrowIfNotStarted();

            var result = storage.Get(id);
            if (result == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return result;
        }

        public void Put(string id, [FromBody] Value value)
        {
            stateRepository.ThrowIfNotStarted();

            storage.Set(id, value);
        }

        private IPEndPoint[] GetShardEndpoint(string id)
        {
            if (configuration.Topology == null)
                return new[] {configuration.CurrentNodeEndpoint};

            return configuration.Topology.First();
        }
    }
}