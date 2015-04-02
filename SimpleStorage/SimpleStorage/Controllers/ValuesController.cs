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

        public ValuesController(IStorage storage)
        {
            this.storage = storage;
        }

        // GET api/values 
        public IEnumerable<ValueWithId> Get()
        {
            return storage.GetAll().ToArray();
        }

        // GET api/values/5 
        public Value Get(string id)
        {
            Value result = storage.Get(id);
            if (result == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return result;
        }

        // PUT api/values/5
        public void Put(string id, [FromBody] Value value)
        {
            storage.Set(id, value);
        }

        // DELETE api/values/5 
        public void Delete(string id)
        {
            bool result = storage.Delete(id);
            if (!result)
                throw new HttpResponseException(HttpStatusCode.NotFound);
        }
    }
}