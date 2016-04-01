using System.Collections.Generic;

namespace System.Web.Http.Routing
{
    /// <summary>
    /// Provides information about a route.
    /// </summary>
    public interface IHttpRouteData
    {
        /// <summary>
        /// Gets the object that represents the route.
        /// </summary>
        /// 
        /// <returns>
        /// The object that represents the route.
        /// </returns>
        IHttpRoute Route { get; }

        /// <summary>
        /// Gets a collection of URL parameter values and default values for the route.
        /// </summary>
        /// 
        /// <returns>
        /// The values that are parsed from the URL and from default values.
        /// </returns>
        IDictionary<string, object> Values { get; }
    }
}
