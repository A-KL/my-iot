using System.Collections.Generic;
using System.Net.Http;

namespace System.Web.Http.Routing
{
    public interface IHttpRoute
    {
        /// <summary>
        /// Gets the route template describing the URI pattern to match against.
        /// </summary>
        /// 
        /// <returns>
        /// The route template.
        /// </returns>
        string RouteTemplate { get; }

        /// <summary>
        /// Gets the default values for route parameters if not provided by the incoming <see cref="T:System.Net.Http.HttpRequestMessage"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The default values for route parameters.
        /// </returns>
        IDictionary<string, object> Defaults { get; }

        /// <summary>
        /// Gets the constraints for the route parameters.
        /// </summary>
        /// 
        /// <returns>
        /// The constraints for the route parameters.
        /// </returns>
        IDictionary<string, object> Constraints { get; }

        /// <summary>
        /// Gets any additional data tokens not used directly to determine whether a route matches an incoming <see cref="T:System.Net.Http.HttpRequestMessage"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The additional data tokens.
        /// </returns>
        IDictionary<string, object> DataTokens { get; }

        /// <summary>
        /// Gets the message handler that will be the recipient of the request.
        /// </summary>
        /// 
        /// <returns>
        /// The message handler.
        /// </returns>
        //HttpMessageHandler Handler { get; }

        /// <summary>
        /// Determine whether this route is a match for the incoming request by looking up the &lt;see cref="!:IRouteData" /&gt; for the route.
        /// </summary>
        /// 
        /// <returns>
        /// The &lt;see cref="!:RouteData" /&gt; for a route if matches; otherwise null.
        /// </returns>
        /// <param name="virtualPathRoot">The virtual path root.</param><param name="request">The request.</param>
        IHttpRouteData GetRouteData(string virtualPathRoot, HttpRequestMessage request);

        /// <summary>
        /// Gets a virtual path data based on the route and the values provided.
        /// </summary>
        /// 
        /// <returns>
        /// The virtual path data.
        /// </returns>
        /// <param name="request">The request message.</param><param name="values">The values.</param>
        IHttpVirtualPathData GetVirtualPath(HttpRequestMessage request, IDictionary<string, object> values);
    }
}
