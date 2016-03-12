using System.Web.Http;
using SimpleStorage.Infrastructure;

namespace SimpleStorage.Controllers
{
    public class ServiceController : ApiController
    {
        private readonly IStateRepository stateRepository;

        public ServiceController(IStateRepository stateRepository)
        {
            this.stateRepository = stateRepository;
        }

        [HttpPost]
        public void Start()
        {
            stateRepository.SetState(State.Started);
        }

        [HttpPost]
        public void Stop()
        {
            stateRepository.SetState(State.Stopped);
        }
    }
}