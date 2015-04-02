using System.Collections.Generic;
using System.Web.Http;

namespace SimpleStorage.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values 
        public IEnumerable<string> Get()
        {
            return new[] {"value1", "value2"};
        }

        // GET api/values/5 
        public string Get(string id)
        {
            return "value";
        }

        // PUT api/values/5
        public void Put(string id, [FromBody] string value)
        {
        }

        // DELETE api/values/5 
        public void Delete(string id)
        {
        }
    }
}