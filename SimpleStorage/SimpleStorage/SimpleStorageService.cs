using System;
using Microsoft.Owin.Hosting;
using StructureMap;

namespace SimpleStorage
{
    public static class SimpleStorageService
    {
        public static IDisposable Start(string url, IContainer container)
        {
            var configurator = container.GetInstance<ApplicationConfigurator>();
            var startOptions = new StartOptions(url);
            return WebApp.Start(startOptions, configurator.Configure);
        }
    }
}