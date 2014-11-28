using System.Web.Http;
using Owin;

namespace Service
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration { DependencyResolver = new DependencyResolver(IoCFactory.GetContainer()) };
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });
            appBuilder.UseWebApi(config);
        } 
    }
}