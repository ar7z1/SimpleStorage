using System.Threading;
using System.Threading.Tasks;

namespace SimpleStorage.Infrastructure.Polling
{
    public interface IOperationLogSynchronizer
    {
        Task Synchronize(CancellationToken token);
    }
}