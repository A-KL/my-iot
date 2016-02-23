using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace Griffin.Networking.Web.Handlers
{
    using System.Threading.Tasks;
    using Griffin.Networking.Protocol.Http.Protocol;

    public class WebApiHandler : RouteHandler
    {
        private readonly IList<Type> controllersTypes = new List<Type>();
        //private readonly IGrouping<Type, string> controllers;

        public WebApiHandler(Assembly controllersAssembly)
        {
            this.controllersTypes = (from assemblyType in controllersAssembly.GetTypes()
                                     where typeof(ApiController).IsAssignableFrom(assemblyType)
                                     select assemblyType)
                                     .ToList();
        }

        public override Task<IResponse> ExecuteAsync(string localPath, IRequest request)
        {
            return null;
        }
    }
}
