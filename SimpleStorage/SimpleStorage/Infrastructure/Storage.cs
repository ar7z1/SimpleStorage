using System.Collections.Generic;
using System.Linq;
using Domain;

namespace SimpleStorage.Infrastructure
{
    public class Storage : IStorage
    {
        private readonly IDictionary<string, Value> internalStorage = new Dictionary<string, Value>();
        private readonly IOperationLog operationLog;

        public Storage(IOperationLog operationLog)
        {
            this.operationLog = operationLog;
        }

        public IEnumerable<ValueWithId> GetAll()
        {
            lock (internalStorage)
                return internalStorage.Select(p => new ValueWithId {Id = p.Key, Value = p.Value});
        }

        public Value Get(string id)
        {
            lock (internalStorage)
                return internalStorage.ContainsKey(id) ? internalStorage[id] : null;
        }

        public bool Set(string id, Value value)
        {
            operationLog.Add(new Operation
            {
                Id = id,
                Type = OperationType.Put,
                Value = value
            });
            lock (internalStorage)
                internalStorage[id] = value;
            return true;
        }

        public bool Delete(string id)
        {
            operationLog.Add(new Operation
            {
                Id = id,
                Type = OperationType.Delete
            });
            lock (internalStorage)
                return internalStorage.Remove(id);
        }
    }
}