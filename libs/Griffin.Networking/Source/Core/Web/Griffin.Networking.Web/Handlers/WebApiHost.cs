namespace Griffin.Networking.Web.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Web.Http;

    public static class IListExtensions
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
        }

        public static void MergeRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                if (list.Contains(item))
                {
                    continue;
                }
                list.Add(item);
            }
        }
    }

    public class WebApiHost : IDisposable
    {
        private IList<ApiControllerInfo> controllers;

        private readonly Assembly assembly;

        private IList<string> routes;

        public IEnumerable<string> Routes
        {
            get { return this.routes; }
        }

        public WebApiHost(Assembly assembly, IDictionary<string, string> map = null)
        {
            this.assembly = assembly;

            if (map != null)
            {
                this.routes = new List<string>();
                this.routes.AddRange(map.Values);
            }
        }

        public void Init()
        {
            this.controllers = ApiControllerInfo.Lookup<ApiController>(this.assembly);

            if (this.routes != null)
            {
                return;
            }

            this.routes = new List<string>();

            foreach (var controllerInfo in this.controllers)
            {
                this.routes.MergeRange(controllerInfo.AttributeRoutes);
            }
        }

        public void Dispose()
        {
            
        }

        private string GetMatchedRoute(string localPath)
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
    }
}
