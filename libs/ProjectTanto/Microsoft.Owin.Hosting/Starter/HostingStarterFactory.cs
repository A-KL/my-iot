namespace Microsoft.Owin.Hosting.Starter
{
    using System;
    using System.Linq;
    using System.IO;
    using System.Reflection;
    using System.Collections.Generic;

    /// <summary>
    /// Selects from known hosting starters, or detects additional providers via convention.
    /// </summary>
    public class HostingStarterFactory : IHostingStarterFactory
    {
        private readonly IHostingStarterActivator hostingStarterActivator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostingStarterActivator"></param>
        public HostingStarterFactory(IHostingStarterActivator hostingStarterActivator)
        {
            this.hostingStarterActivator = hostingStarterActivator;
        }

        /// <summary>
        /// Selects from known hosting starters, or detects additional providers via convention.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual IHostingStarter Create(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return hostingStarterActivator.Activate(typeof(DirectHostingStarter));
            }
            //if (name == "Domain")
            //{
            //    return hostingStarterActivator.Activate(typeof(DomainHostingStarter));
            //}

            // TODO: Is the attribute necessary? Can we load this using just a naming convention like we do for App and ServerFactory?
            var hostingStarterType = LoadProvider(name)
                .GetCustomAttributes(typeof(HostingStarterAttribute))
                .OfType<HostingStarterAttribute>()
                .Select(attribute => attribute.HostingStarterType)
                .SingleOrDefault();

            return hostingStarterActivator.Activate(hostingStarterType);
        }

        private static Assembly LoadProvider(params string[] names)
        {
            var innerExceptions = new List<Exception>();

            foreach (var name in names)
            {
                try
                {
                    return Assembly.Load(new AssemblyName(name));
                }
                catch (FileNotFoundException ex)
                {
                    innerExceptions.Add(ex);
                }
                catch (FileLoadException ex)
                {
                    innerExceptions.Add(ex);
                }
                catch (BadImageFormatException ex)
                {
                    innerExceptions.Add(ex);
                }
            }

            throw new AggregateException(innerExceptions);
        }
    }
}
