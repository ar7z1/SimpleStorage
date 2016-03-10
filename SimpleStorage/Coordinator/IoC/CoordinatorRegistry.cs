using System.Web.Http.Controllers;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap;

namespace Coordinator.IoC
{
    public class CoordinatorRegistry : Registry
    {
        public CoordinatorRegistry()
        {
            Scan(p =>
            {
                p.TheCallingAssembly();
                p.RegisterConcreteTypesAgainstTheFirstInterface();
                p.AddAllTypesOf<IHttpController>();
            });
        }
    }
}