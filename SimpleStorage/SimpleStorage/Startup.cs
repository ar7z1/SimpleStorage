using System.Web.Http;
using Owin;
using SimpleStorage.Controllers;
using SimpleStorage.IoC;

namespace SimpleStorage
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration {DependencyResolver = new DependencyResolver(IoCFactory.GetContainer())};
            config.Routes.MapHttpRoute("AdminApi", "api/admin/{action}", new {controller = "Admin"});
            config.Routes.MapHttpRoute("OperationLogApi", "api/operations", new {controller = "Operations", action = "Get"});
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new {id = RouteParameter.Optional});
            appBuilder.Use<LogMiddleware>();
            appBuilder.UseWebApi(config);
        }
    }
}