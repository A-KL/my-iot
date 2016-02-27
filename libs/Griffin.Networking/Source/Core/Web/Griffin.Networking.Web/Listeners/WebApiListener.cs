namespace Griffin.Networking.Web.Listeners
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using Protocol.Http.Protocol;
    using WebApi;

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
