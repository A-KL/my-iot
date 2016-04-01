namespace System.Web.Http.Routing
{
    /// <summary>
    /// Defines the properties for HTTP route.
    /// </summary>
    public interface IHttpVirtualPathData
    {
        /// <summary>
        /// Gets the HTTP route.
        /// </summary>
        /// 
        /// <returns>
        /// The HTTP route.
        /// </returns>
        IHttpRoute Route { get; }

        /// <summary>
        /// Gets the URI that represents the virtual path of the current HTTP route.
        /// </summary>
        /// 
        /// <returns>
        /// The URI that represents the virtual path of the current HTTP route.
        /// </returns>
        string VirtualPath { get; set; }
    }
}
