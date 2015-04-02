using System.Collections.Generic;
using Domain;

namespace Client
{
    public interface ISimpleStorageClient
    {
        void Put(string id, Value value);
        IEnumerable<ValueWithId> GetAll();
        Value Get(string id);
        void Delete(string id);
    }
}