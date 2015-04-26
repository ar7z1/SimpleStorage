using System.Net;
using System.Web.Http;
using SimpleStorage.Infrastructure;

namespace SimpleStorage.Controllers
{
    public static class ControllerHelpers
    {
        public static void ThrowIfNotStarted(this IStateRepository stateRepository)
        {
            if (stateRepository.GetState() != State.Started)
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
        }
    }
}