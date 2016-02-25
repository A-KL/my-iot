namespace Griffin.Networking.Web.Handlers
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using Protocol.Http.Protocol;
    using WebApi;

    public class WebApiHandler : RouteHandler
    {
        private readonly WebApiHost host;

        public override IEnumerable<string> Routes
        {
            get { return this.host.Routes; }
        }

        public WebApiHandler(Assembly assembly, IDictionary<string, string> map = null)
        {
            this.host = new WebApiHost(assembly, map);
            this.host.Init();
        }

        public override Task<IResponse> ExecuteAsync(IRequest request)
        {
            this.host.Invoke(request);

            return null;
        }

        private static IDictionary<string, ApiControllerInfo> GetRoutesFromRoutingAttributes(IEnumerable<ApiControllerInfo> controllers)
        {
            var result = new Dictionary<string, ApiControllerInfo>();

            foreach (var apiControllerInfo in controllers)
            {
                foreach (var attributeRoute in apiControllerInfo.AttributeRoutes)
                {
                    if (result.ContainsKey(attributeRoute))
                    {
                        continue;
                    }
                    result.Add(attributeRoute, apiControllerInfo);
                }
            }

            return result;
        }
    }
}
