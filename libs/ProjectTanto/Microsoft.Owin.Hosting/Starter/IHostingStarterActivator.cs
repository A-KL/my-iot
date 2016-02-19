namespace Microsoft.Owin.Hosting.Starter
{
    using System;

    /// <summary>
    /// Instantiates instances of the IHostingStarter.
    /// </summary>
    public interface IHostingStarterActivator
    {
        /// <summary>
        /// Instantiates instances of the IHostingStarter.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IHostingStarter Activate(Type type);
    }
}
