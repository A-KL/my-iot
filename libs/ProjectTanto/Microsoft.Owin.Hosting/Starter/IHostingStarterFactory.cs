namespace Microsoft.Owin.Hosting.Starter
{
    /// <summary>
    /// Creates a IHostingStarter for the given identifier.
    /// </summary>
    public interface IHostingStarterFactory
    {
        /// <summary>
        /// Creates a IHostingStarter for the given identifier.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IHostingStarter Create(string name);
    }
}
