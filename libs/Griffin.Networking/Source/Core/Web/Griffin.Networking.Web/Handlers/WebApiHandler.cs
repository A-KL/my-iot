using System.Collections.Generic;
using System.Reflection;
using System.Web.Http;

namespace Griffin.Networking.Web.Handlers
{
    using System.Threading.Tasks;
    using Griffin.Networking.Protocol.Http.Protocol;

    public class WebApiHandler : RouteHandler
    {
        private readonly IList<ApiControllerInfo> controllers;

        public WebApiHandler(Assembly assembly)
        {
            this.controllers = ApiControllerInfo.Lookup<ApiController>(assembly);
        }

        public override Task<IResponse> ExecuteAsync(string localPath, IRequest request)
        {
            return null;
        }
    }
}
