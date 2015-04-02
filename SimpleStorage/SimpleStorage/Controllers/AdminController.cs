using System.Web.Http;
using SimpleStorage.Infrastructure;

namespace SimpleStorage.Controllers
{
    public class AdminController : ApiController
    {
        private readonly IStateRepository stateRepository;

        public AdminController(IStateRepository stateRepository)
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