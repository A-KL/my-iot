using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Griffin.Networking.Protocol.Http.Protocol;
using Griffin.Networking.Web.Listeners.WebApi;

namespace Griffin.Networking.Web.Listeners
{
    public class WebApiListener : RouteListener
    {
        private readonly WebApiHost host;

        public override bool IsListeningTo(Uri uri)
        {
            return true;
        }

        public WebApiListener(Assembly assembly, IDictionary<string, string> map = null)
        {
            this.host = new WebApiHost(assembly, map);
            this.host.Init();
        }

        public override Task<IResponse> ExecuteAsync(IRequest request)
        {
            return Task.FromResult(this.host.Invoke(request));
        }
    }
}
