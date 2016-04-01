//using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Routing;

namespace System.Web.Http.Controllers
{
    /// <summary>
    /// Represents the context associated with a request.
    /// </summary>
    public class HttpRequestContext
    {
        /// <summary>
        /// Gets or sets the client certificate.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2"/>.
        /// </returns>
        //public virtual X509Certificate2 ClientCertificate { get; set; }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Web.Http.HttpConfiguration"/>.
        /// </returns>
        public virtual HttpConfiguration Configuration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether error details, such as exception messages and stack traces, should be included in the response for this request.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Boolean"/>.
        /// </returns>
        public virtual bool IncludeErrorDetail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the request originates from a local address.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Boolean"/>.
        /// </returns>
        public virtual bool IsLocal { get; set; }

        /// <summary>
        /// .Gets or sets the principal
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Security.Principal.IPrincipal"/>.
        /// </returns>
        public virtual IPrincipal Principal { get; set; }

        /// <summary>
        /// Gets or sets the route data.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Web.Http.Routing.IHttpRouteData"/>.
        /// </returns>
        public virtual IHttpRouteData RouteData { get; set; }

        /// <summary>
        /// Gets or sets the factory used to generate URLs to other APIs.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Web.Http.Routing.UrlHelper"/>.
        /// </returns>
        public virtual UrlHelper Url { get; set; }

        /// <summary>
        /// Gets or sets the virtual path root.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.String"/>.
        /// </returns>
        public virtual string VirtualPathRoot { get; set; }
    }
}
