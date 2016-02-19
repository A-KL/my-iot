namespace Microsoft.Owin.Hosting.Starter
{
    using System;

    /// <summary>
    /// Determines the which IHostingStarter instance to use via the IHostingSterterFactory.
    /// </summary>
    public class HostingStarter : IHostingStarter
    {
        private readonly IHostingStarterFactory hostingStarterFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostingStarterFactory"></param>
        public HostingStarter(IHostingStarterFactory hostingStarterFactory)
        {
            this.hostingStarterFactory = hostingStarterFactory;
        }

        /// <summary>
        /// Determines the which IHostingStarter instance to use via the IHostingSterterFactory.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public virtual IDisposable Start(StartOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            string boot;
            options.Settings.TryGetValue("boot", out boot);

            var hostingStarter = hostingStarterFactory.Create(boot);

            return hostingStarter.Start(options);
        }
    }
}
