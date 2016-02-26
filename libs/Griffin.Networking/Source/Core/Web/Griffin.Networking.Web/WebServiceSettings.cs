using System.Collections.Generic;
using Griffin.Networking.Web.Listeners.WebApi;

namespace Griffin.Networking.Web
{
    public class WebServiceSettings
    {
        private readonly IList<RouteListener> listeners = new List<RouteListener>();

        public string DefaultPath { get; set; }

        public IList<RouteListener> Listeners
        {
            get { return this.listeners; }
        }
    }
}