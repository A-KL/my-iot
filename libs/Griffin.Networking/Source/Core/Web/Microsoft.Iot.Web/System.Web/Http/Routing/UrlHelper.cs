using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Properties;

namespace System.Web.Http.Routing
{
    /// <summary>
    /// Represents a factory for creating URLs.
    /// </summary>
    public class UrlHelper
    {
        private HttpRequestMessage _request;

        /// <summary>
        /// Gets or sets the <see cref="T:System.Net.Http.HttpRequestMessage"/> of the current <see cref="T:System.Web.Http.Routing.UrlHelper"/> instance.
        /// </summary>
        /// 
        /// <returns>
        /// The <see cref="T:System.Net.Http.HttpRequestMessage"/> of the current instance.
        /// </returns>
        public HttpRequestMessage Request
        {
            get
            {
                return this._request;
            }
            set
            {
                if (value == null)
                    throw Error.PropertyNull();
                this._request = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Http.Routing.UrlHelper"/> class.
        /// </summary>
        public UrlHelper()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Http.Routing.UrlHelper"/> class.
        /// </summary>
        /// <param name="request">The HTTP request for this instance.</param>
        public UrlHelper(HttpRequestMessage request)
        {
            if (request == null)
                throw Error.ArgumentNull("request");
            this.Request = request;
        }

        /// <summary>
        /// Creates an absolute URL using the specified path.
        /// </summary>
        /// 
        /// <returns>
        /// The generated URL.
        /// </returns>
        /// <param name="path">The URL path, which may be a relative URL, a rooted URL, or a virtual path.</param>
        public virtual string Content(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw Error.ArgumentNullOrEmpty("path");
            }

            if (this.Request == null)
            {
                throw Error.InvalidOperation("SRResources.RequestIsNull", (object)"UrlHelper");
            }

            if (!path.StartsWith("~/", StringComparison.Ordinal))
            {
                return new Uri(this.Request.RequestUri, path).AbsoluteUri;
            }

            HttpRequestContext requestContext = null;// HttpRequestMessageExtensions.GetRequestContext(this.Request);

            string str;
            if (requestContext != null)
            {
                str = requestContext.VirtualPathRoot;
            }
            else
            {
                HttpConfiguration configuration = HttpRequestMessageExtensions.GetConfiguration(this.Request);

                if (configuration == null)
                    throw Error.InvalidOperation("SRResources.HttpRequestMessageExtensions_NoConfiguration");

                str = configuration.VirtualPathRoot;
            }
            if (str == null)
                str = "/";
            if (!str.StartsWith("/", StringComparison.Ordinal))
                str = "/" + str;
            if (!str.EndsWith("/", StringComparison.Ordinal))
                str += "/";
            return new Uri(this.Request.RequestUri, str + path.Substring("~/".Length)).AbsoluteUri;
        }

        /// <summary>
        /// Returns the route for the <see cref="T:System.Web.Http.Routing.UrlHelper"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The route for the <see cref="T:System.Web.Http.Routing.UrlHelper"/>.
        /// </returns>
        /// <param name="routeName">The name of the route.</param><param name="routeValues">The route values.</param>
        //public virtual string Route(string routeName, object routeValues)
        //{
        //    return this.Route(routeName, (IDictionary<string, object>)new HttpRouteValueDictionary(routeValues));
        //}

        /// <summary>
        /// Returns the route for the <see cref="T:System.Web.Http.Routing.UrlHelper"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The route for the <see cref="T:System.Web.Http.Routing.UrlHelper"/>.
        /// </returns>
        /// <param name="routeName">The name of the route.</param><param name="routeValues">A list of route values.</param>
        //public virtual string Route(string routeName, IDictionary<string, object> routeValues)
        //{
        //    return UrlHelper.GetVirtualPath(this.Request, routeName, routeValues);
        //}

        /// <summary>
        /// Returns a link for the specified route.
        /// </summary>
        /// 
        /// <returns>
        /// A link for the specified route.
        /// </returns>
        /// <param name="routeName">The name of the route.</param><param name="routeValues">A route value.</param>
        //public virtual string Link(string routeName, object routeValues)
        //{
        //    return this.Link(routeName, (IDictionary<string, object>)new HttpRouteValueDictionary(routeValues));
        //}

        /// <summary>
        /// Returns a link for the specified route.
        /// </summary>
        /// 
        /// <returns>
        /// A link for the specified route.
        /// </returns>
        /// <param name="routeName">The name of the route.</param><param name="routeValues">An object that contains the parameters for a route.</param>
        //public virtual string Link(string routeName, IDictionary<string, object> routeValues)
        //{
        //    string relativeUri = this.Route(routeName, routeValues);
        //    if (!string.IsNullOrEmpty(relativeUri))
        //        relativeUri = new Uri(this.Request.RequestUri, relativeUri).AbsoluteUri;
        //    return relativeUri;
        //}

        //private static string GetVirtualPath(HttpRequestMessage request, string routeName, IDictionary<string, object> routeValues)
        //{
        //    if (routeValues == null)
        //    {
        //        routeValues = (IDictionary<string, object>)new HttpRouteValueDictionary();
        //        routeValues.Add(HttpRoute.HttpRouteKey, (object)true);
        //    }
        //    else
        //    {
        //        routeValues = (IDictionary<string, object>)new HttpRouteValueDictionary(routeValues);
        //        if (!routeValues.ContainsKey(HttpRoute.HttpRouteKey))
        //            routeValues.Add(HttpRoute.HttpRouteKey, (object)true);
        //    }
        //    HttpConfiguration configuration = HttpRequestMessageExtensions.GetConfiguration(request);
        //    if (configuration == null)
        //        throw Error.InvalidOperation("SRResources.HttpRequestMessageExtensions_NoConfiguration");
        //    IHttpVirtualPathData virtualPath = configuration.Routes.GetVirtualPath(request, routeName, routeValues);
        //    if (virtualPath == null)
        //        return (string)null;
        //    return virtualPath.VirtualPath;
        //}
    }
}
