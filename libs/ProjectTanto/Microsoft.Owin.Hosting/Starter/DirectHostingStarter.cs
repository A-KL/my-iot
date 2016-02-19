namespace Microsoft.Owin.Hosting.Starter
{
    using System;
    using Microsoft.Owin.Hosting.Engine;

    /// <summary>
    /// Executes the IHostingEngine without making any changes to the current execution environment.
    /// </summary>
    public class DirectHostingStarter : IHostingStarter
    {
        private readonly IHostingEngine engine;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="engine"></param>
        public DirectHostingStarter(IHostingEngine engine)
        {
            this.engine = engine;
        }

        /// <summary>
        /// Executes the IHostingEngine without making any changes to the current execution environment.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public virtual IDisposable Start(StartOptions options)
        {
            return engine.Start(new StartContext(options));
        }
    }
}
