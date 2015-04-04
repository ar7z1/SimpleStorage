using System.Web.Http;
using SimpleStorage.Infrastructure;

namespace SimpleStorage.Controllers
{
    public class AdminController : ApiController
    {
        private readonly IOperationLog operationLog;
        private readonly IStateRepository stateRepository;
        private readonly IStorage storage;

        public AdminController(IStateRepository stateRepository, IStorage storage, IOperationLog operationLog)
        {
            this.stateRepository = stateRepository;
            this.storage = storage;
            this.operationLog = operationLog;
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

        [HttpPost]
        public void RemoveAllData()
        {
            operationLog.RemoveAll();
            storage.RemoveAll();
        }
    }
}