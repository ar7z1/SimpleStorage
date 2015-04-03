using System;
using System.Collections.Generic;
using Domain;

namespace SimpleStorage.Infrastructure
{
    public class OperationLog : IOperationLog
    {
        private List<Operation> log = new List<Operation>();


        public void Add(Operation operation)
        {
            lock (log)
                log.Add(operation);
        }

        public IEnumerable<Operation> Read(int position, int count)
        {
            lock (log)
                return log.GetRange(position, Math.Min(count, log.Count - position));
        }
    }
}