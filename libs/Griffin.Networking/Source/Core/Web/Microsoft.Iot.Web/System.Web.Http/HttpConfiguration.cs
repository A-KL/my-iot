using System.Collections.Generic;
using System.Web.Http;
using Microsoft.Iot.Web;

namespace System.Web.Http
{
    public class HttpConfiguration
    {
        //private readonly HttpRouteCollection routes;

        private readonly IList<RouteListener> listeners = new List<RouteListener>();

        public string DefaultPath { get; set; }

        public IDependencyResolver DependencyResolver { get; set; }

        public IList<RouteListener> Listeners
        {
            get { return this.listeners; }
        }
    }
}