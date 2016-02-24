using System.Text.RegularExpressions;
using Windows.UI.Xaml;

namespace Griffin.Networking.Web.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Web.Http;
    using System.Threading.Tasks;
    using Griffin.Networking.Protocol.Http.Protocol;

    public class WebApiHandler : RouteHandler
    {
        private readonly WebApiHost host;

        public override string Route
        {
            get { return "/api"; }
        }

        //public IEnumerable<string> Routes
        //{
        //    get { return this.routesMap.Keys; }
        //}

        public WebApiHandler(Assembly assembly, IDictionary<string, string> map = null)
        {
            this.host = new WebApiHost(assembly, map);
        }

        public override Task<IResponse> ExecuteAsync(IRequest request)
        {
            var requsetPath = request.Uri.LocalPath;

            foreach (var controllerInfo in this.controllers)
            {
                foreach (var attributeRoute in controllerInfo.AttributeRoutes)
                {
                    IDictionary<string, string> variables;

                    if (MatchUriToRoute(request.Uri.LocalPath, attributeRoute, out variables))
                    {
                        
                    }
                }                
            }

            return null;
        }

        private static bool MatchUriToRoute(string localPath, string route, out IDictionary<string, string> variables)
        {
            variables = null;

            var routeUriSegments = route.TrimStart('/').Split('/');
            var uriSegments = localPath.TrimStart('/').Split('/');

            if (routeUriSegments.Length != uriSegments.Length)
            {
                return false;
            }

            variables = new Dictionary<string, string>();

            for (var i = 0; i < uriSegments.Length; ++i)
            {
                if (uriSegments[i].Equals(routeUriSegments[i], StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var match = Regex.Match(routeUriSegments[i], @"{(\w+)}", RegexOptions.IgnoreCase);
                if (!match.Success)
                {
                    return false;
                }

                variables.Add(match.Groups[1].Value, uriSegments[i]);
            }

            return true;
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
