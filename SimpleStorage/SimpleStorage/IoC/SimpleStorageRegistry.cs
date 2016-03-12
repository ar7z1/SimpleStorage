using System.Collections.Generic;
using System.Web.Http.Controllers;
using Domain;
using SimpleStorage.Infrastructure;
using StructureMap;
using StructureMap.Graph;

namespace SimpleStorage.IoC
{
    public class SimpleStorageRegistry : Registry
    {
        public SimpleStorageRegistry()
        {
            Scan(p =>
            {
                p.TheCallingAssembly();
                p.WithDefaultConventions();
                p.AddAllTypesOf<IHttpController>();
            });
            For<IStorage>().Singleton();
            For<IStateRepository>().Singleton();
            For<IOperationLog>().Singleton();
            For<OperationLogSynchronizer>().Singleton();
            For<IComparer<Value>>().Use<ValueComparer>().Singleton();
        }
    }
}