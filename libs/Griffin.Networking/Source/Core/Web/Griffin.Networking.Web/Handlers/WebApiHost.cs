using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace Griffin.Networking.Web.Handlers
{
    public class WebApiHost : IDisposable
    {
        private IList<ApiControllerInfo> controllers;

        private readonly Assembly assembly;

        public WebApiHost(Assembly assembly, IDictionary<string, string> map = null)
        {
            this.assembly = assembly;
        }

        public void Init()
        {
            this.controllers = ApiControllerInfo.Lookup<ApiController>(this.assembly);
        }

        public string GetMatchedRoute(string localPath)
        {
            foreach (var controllerInfo in this.controllers)
            {
                foreach (var attributeRoute in controllerInfo.AttributeRoutes)
                {
                    IDictionary<string, string> variables;

                    if (MatchUriToRoute(localPath, attributeRoute, out variables))
                    {

                    }
                }
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
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
    }
}
