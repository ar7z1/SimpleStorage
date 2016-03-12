using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace SimpleStorage.Infrastructure
{
    public class OperationLogSynchronizer
    {
        private readonly IStorage storage;
        private readonly SimpleStorageConfiguration configuration;

        public OperationLogSynchronizer(SimpleStorageConfiguration configuration, IStorage storage)
        {
            this.configuration = configuration;
            this.storage = storage;
        }

        public Task Synchronize(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => SynchronizationAction(cancellationToken), cancellationToken);
        }

        private void SynchronizationAction(CancellationToken token)
        {
            if (configuration.Master == null)
                return;

            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                Thread.Sleep(1000);
            }
        }
    }
}