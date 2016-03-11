using Domain;
using System.Threading.Tasks;
using System.Threading;

namespace SimpleStorage.Infrastructure
{
    public class OperationLogSynchronizer
    {
        private SimpleStorageConfiguration configuration;

        public OperationLogSynchronizer(SimpleStorageConfiguration configuration)
        {
            this.configuration = configuration;
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
                if (token.IsCancellationRequested) {
                    return;
                }
                Thread.Sleep(1000);
            }
        }
    }
}