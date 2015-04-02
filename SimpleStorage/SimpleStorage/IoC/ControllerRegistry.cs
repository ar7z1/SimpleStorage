using System.Web.Http.Controllers;
using SimpleStorage.Infrastructure;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace SimpleStorage.IoC
{
    public class ControllerRegistry : Registry
    {
        public ControllerRegistry()
        {
            Scan(p =>
            {
                p.TheCallingAssembly();
                p.RegisterConcreteTypesAgainstTheFirstInterface();
                p.AddAllTypesOf<IHttpController>();
            });
            For<IStorage>().Singleton();
            For<IStateRepository>().Singleton();
        }
    }
}