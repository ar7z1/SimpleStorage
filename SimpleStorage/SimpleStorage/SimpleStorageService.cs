using System;
using Domain;
using Microsoft.Owin.Hosting;
using SimpleStorage.IoC;
using StructureMap;

namespace SimpleStorage
{
    public static class SimpleStorageService
    {
        public static IDisposable Start(SimpleStorageConfiguration configuration)
        {
            var container = new Container(new SimpleStorageRegistry());
            container.Configure(c => c.For<SimpleStorageConfiguration>().Use(configuration));

            var configurator = container.GetInstance<ApplicationConfigurator>();
            var startOptions = new StartOptions(string.Format("http://+:{0}/", configuration.Port));
            return WebApp.Start(startOptions, configurator.Configure);
        }
    }
}