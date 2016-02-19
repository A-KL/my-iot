using System;
using Microsoft.Owin.Hosting.Services;

namespace Microsoft.Owin.Hosting.Starter
{
    /// <summary>
    /// Instantiates instances of the IHostingStarter.
    /// </summary>
    public class HostingStarterActivator : IHostingStarterActivator
    {
        private readonly IServiceProvider services;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public HostingStarterActivator(IServiceProvider services)
        {
            this.services = services;
        }

        /// <summary>
        /// Instantiates instances of the IHostingStarter.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual IHostingStarter Activate(Type type)
        {
            var starter = ActivatorUtilities.GetServiceOrCreateInstance(services, type);

            return (IHostingStarter)starter;
        }
    }
}
