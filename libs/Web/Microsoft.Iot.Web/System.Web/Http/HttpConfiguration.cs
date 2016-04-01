using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Web.Http.Dependencies;
using System.Web.Http.ModelBinding;
using Microsoft.Iot.Web;

namespace System.Web.Http
{
    public class HttpConfiguration
    {
        private readonly HttpRouteCollection _routes;
        private readonly ConcurrentDictionary<object, object> _properties = new ConcurrentDictionary<object, object>();
        private readonly MediaTypeFormatterCollection _formatters;

        private IDependencyResolver _dependencyResolver = EmptyResolver.Instance;
        private readonly IList<RouteListener> listeners = new List<RouteListener>();

        public string DefaultPath { get; set; }
    
        public IList<RouteListener> Listeners
        {
            get { return this.listeners; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Http.HttpRouteCollection"/> associated with this <see cref="T:System.Web.Http.HttpConfiguration"/> instance.
        /// </summary>
        /// 
        /// <returns>
        /// The <see cref="T:System.Web.Http.HttpRouteCollection"/>.
        /// </returns>
        public HttpRouteCollection Routes
        {
            get
            {
                return this._routes;
            }
        }

        /// <summary>
        /// Gets the properties associated with this instance.
        /// </summary>
        /// 
        /// <returns>
        /// The <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/>that contains the properties.
        /// </returns>
        public ConcurrentDictionary<object, object> Properties
        {
            get
            {
                return this._properties;
            }
        }

        /// <summary>
        /// Gets the root virtual path.
        /// </summary>
        /// 
        /// <returns>
        /// The root virtual path.
        /// </returns>
        public string VirtualPathRoot
        {
            get
            {
                return this._routes.VirtualPathRoot;
            }
        }

        /// <summary>
        /// Gets or sets the dependency resolver associated with thisinstance.
        /// </summary>
        /// 
        /// <returns>
        /// The dependency resolver.
        /// </returns>
        public IDependencyResolver DependencyResolver
        {
            get
            {
                return this._dependencyResolver;
            }
            set
            {
                if (value == null)
                {
                    throw Error.PropertyNull();
                }
                this._dependencyResolver = value;
            }
        }

        /// <summary>
        /// Gets the media-type formatters for this instance.
        /// </summary>
        /// 
        /// <returns>
        /// A collection of <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter"/> objects.
        /// </returns>
        public MediaTypeFormatterCollection Formatters
        {
            get
            {
                return this._formatters;
            }
        }

        /// <summary>
        /// Gets the container of default services associated with this instance.
        /// </summary>
        /// 
        /// <returns>
        /// The <see cref="T:System.Web.Http.Controllers.ServicesContainer"/> that contains the default services for this instance.
        /// </returns>
        //public ServicesContainer Services { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Http.HttpConfiguration"/> class.
        /// </summary>
        public HttpConfiguration()
          : this(new HttpRouteCollection(string.Empty))
        {
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Http.HttpConfiguration"/> class with an HTTP route collection.
        /// </summary>
        /// <param name="routes">The HTTP route collection to associate with this instance.</param>
        public HttpConfiguration(HttpRouteCollection routes)
        {
            if (routes == null)
            {
                throw Error.ArgumentNull("routes");
            }
            this._routes = routes;
            this._formatters = HttpConfiguration.DefaultFormatters(this);
            //this.Services = (ServicesContainer)new DefaultServices(this);
            //this.ParameterBindingRules = DefaultActionValueBinder.GetDefaultParameterBinders();
        }

        private static MediaTypeFormatterCollection DefaultFormatters(HttpConfiguration config)
        {
            MediaTypeFormatterCollection formatterCollection = new MediaTypeFormatterCollection();
            formatterCollection.Add(new JQueryMvcFormUrlEncodedFormatter(config));
            return formatterCollection;
        }
    }
}