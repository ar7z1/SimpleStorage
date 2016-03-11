using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Owin;
using SimpleStorage.IoC;
using StructureMap;
using Core.Web;

namespace SimpleStorage
{
	public class ApplicationConfigurator
	{
		private readonly IContainer container;

		public ApplicationConfigurator(IContainer container)
		{
			this.container = container;
		}

		public void Configure(IAppBuilder appBuilder)
		{
			var config = new HttpConfiguration { DependencyResolver = new DependencyResolver(container) };
			config.Routes.MapHttpRoute("ServiceApi", "api/service/{action}", new {controller = "Service"});
			config.Routes.MapHttpRoute("OperationLogApi", "api/operations",
			                                    new {controller = "Operations", action = "Get"});
            config.Routes.MapHttpRoute("ConfigurationApi", "api/configuration",
                                       new {controller = "Configuration", action = "Get"});
			config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new {id = RouteParameter.Optional});
			config.Services.Replace(typeof(IExceptionLogger), new ConsoleExceptionLogger());
			appBuilder.Use<LogMiddleware>();
			appBuilder.UseWebApi(config);
		}
	}
}