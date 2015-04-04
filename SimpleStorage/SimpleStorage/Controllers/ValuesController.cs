using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Client;
using Domain;
using SimpleStorage.Infrastructure;

namespace SimpleStorage.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly IConfiguration configuration;
        private readonly IComparer<Value> valueComparer;
        private readonly InternalClient[] clients;
        private readonly IStateRepository stateRepository;
        private readonly IStorage storage;
        private readonly int successOperationCount;
        private readonly object lockObj;

        public ValuesController(IStorage storage, IStateRepository stateRepository, IConfiguration configuration, ITopology topology, IComparer<Value> valueComparer)
        {
            this.storage = storage;
            this.stateRepository = stateRepository;
            this.configuration = configuration;
            this.valueComparer = valueComparer;
            clients = topology.Replicas.Select(point => new InternalClient(string.Format("http://{0}/", point))).ToArray();
            successOperationCount = (topology.Replicas.Count() + 1)/2 + 1;
        }

        private void CheckState()
        {
            if (stateRepository.GetState() != State.Started)
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
        }

        // GET api/values/5 
        public Value Get(string id)
        {
            CheckState();
            var result = storage.Get(id);
            var success = 1;
            foreach (var client in clients)
            {
                try
                {
                    var candidate = client.Get(id);
                    success += 1;
                    if (result != null && candidate != null)
                    {
                        if (valueComparer.Compare(result, candidate) < 0)
                            result = candidate;
                        continue;
                    }
                    result = result ?? candidate;
                }
                catch { }
            }
            if (success < successOperationCount)
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            if (result == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return result;
        }

        // PUT api/values/5
        public void Put(string id, [FromBody] Value value)
        {
            CheckState();
            storage.Set(id, value);
            var success = 1;
            foreach (var client in clients)
            {
                                                    try
                                                    {
                                                        client.Put(id, value);
                                                        success += 1;
                                                    } catch {}
            }
            if (success < successOperationCount)
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
        }
    }
}