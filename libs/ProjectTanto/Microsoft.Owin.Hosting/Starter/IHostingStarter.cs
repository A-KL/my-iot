namespace Microsoft.Owin.Hosting.Starter
{
    using System;

    /// <summary>
    /// Performs any necessary environment setup prior to executing the IHostingEngine.
    /// </summary>
    public interface IHostingStarter
    {
        /// <summary>
        /// Performs any necessary environment setup prior to executing the IHostingEngine.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        IDisposable Start(StartOptions options);
    }
}
