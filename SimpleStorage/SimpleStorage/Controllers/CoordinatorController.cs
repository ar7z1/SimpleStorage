using System.Web.Http;
using Domain;
using System.Linq;
using System.Net;

namespace Coordinator.Controllers
{
    public class CoordinatorController : ApiController
    {
        private SimpleStorageConfiguration configuration;

        public CoordinatorController(SimpleStorageConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IPEndPoint Get(string id)
        {
            return configuration.CurrentNodeEndpoint;
        }
    }
}