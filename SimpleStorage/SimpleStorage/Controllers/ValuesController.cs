using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Domain;
using SimpleStorage.Infrastructure;

namespace SimpleStorage.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly IStorage storage;
        private readonly IStateRepository stateRepository;

        public ValuesController(IStorage storage, IStateRepository stateRepository)
        {
            this.storage = storage;
            this.stateRepository = stateRepository;
        }

        // GET api/values 
        public IEnumerable<ValueWithId> Get()
        {
            CheckState();
            return storage.GetAll().ToArray();
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
            Value result = storage.Get(id);
            if (result == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return result;
        }

        // PUT api/values/5
        public void Put(string id, [FromBody] Value value)
        {
            CheckState();
            storage.Set(id, value);
        }

        // DELETE api/values/5 
        public void Delete(string id)
        {
            CheckState();
            bool result = storage.Delete(id);
            if (!result)
                throw new HttpResponseException(HttpStatusCode.NotFound);
        }
    }
}