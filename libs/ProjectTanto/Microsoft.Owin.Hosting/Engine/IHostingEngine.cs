namespace Microsoft.Owin.Hosting.Engine
{
    using System;

    /// <summary>
    /// Initializes and starts a web application.
    /// </summary>
    public interface IHostingEngine
    {
        /// <summary>
        /// Initializes and starts a web application.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IDisposable Start(StartContext context);
    }
}
