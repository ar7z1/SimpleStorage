using System.Web.Http.Controllers;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace Service.IoC
{
    public class ControllerRegistry : Registry
    {
        public ControllerRegistry()
        {
            Scan(p =>
            {
                p.TheCallingAssembly();
                p.AddAllTypesOf<IHttpController>();
            });
        }
    }
}