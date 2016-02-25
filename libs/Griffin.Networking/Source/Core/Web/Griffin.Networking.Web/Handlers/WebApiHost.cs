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
        private IDictionary<string, ApiControllerInfo> controllers;

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
            var results = ApiControllerInfo.Lookup<ApiController>(this.assembly);

            if (this.routes != null)
            {
                return;
            }

            this.routes = new List<string>();

            foreach (var controller in results)
            {
                this.controllers.Add(controller.Name, controller);
                this.routes.MergeRange(controller.AttributeRoutes);
            }
        }

        public void Dispose()
        {

        }

        public void Invoke(string localPath)
        {
            foreach (var route in this.routes)
            {
                IDictionary<string, string> variables;

                if (!MatchUriToRoute(localPath, route, out variables))
                {
                    continue;
                }

                if (this.controllers.ContainsKey(variables["controller"]))
                {
                    var controllerInfo = this.controllers["controller"];
                    controllerInfo.Execute();
                }
            }
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
