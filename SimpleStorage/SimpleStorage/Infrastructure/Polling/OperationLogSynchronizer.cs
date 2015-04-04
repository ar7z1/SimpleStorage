using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client;
using Domain;

namespace SimpleStorage.Infrastructure.Polling
{
    public class OperationLogSynchronizer : IOperationLogSynchronizer
    {
        private readonly OperationLogClient operationLogClient;
        private readonly IStorage storage;
        private readonly IMasterConfiguration masterConfiguration;

        private int position;

        public OperationLogSynchronizer(ITopology topology, IStorage storage, IMasterConfiguration masterConfiguration)
        {
            this.storage = storage;
            this.masterConfiguration = masterConfiguration;
            operationLogClient = new OperationLogClient(topology.Replicas.First().ToString());
        }

        public Task Synchronize(CancellationToken cancellationToken)
        {
            Task synchronizationTask = Task.Factory.StartNew(() => SynchronizationAction(cancellationToken), cancellationToken);
            return synchronizationTask;
        }

        private void SynchronizationAction(CancellationToken token)
        {
            if (masterConfiguration.IsMaster)
                return;
            while (true)
            {
                token.ThrowIfCancellationRequested();
                ReadToEnd();
                Thread.Sleep(1000);
            }
        }

        private void ReadToEnd()
        {
            try
            {
                Operation[] operations;
                do
                {
                    operations = operationLogClient.Read(position, 1000).ToArray();
                    foreach (Operation operation in operations)
                        storage.Set(operation.Id, operation.Value);
                    position += operations.Length;
                } while (operations.Length == 1000);
            }
            catch (Exception e)
            {
                Console.WriteLine("Synchronization fail. {0}", e);
            }
        }
    }
}