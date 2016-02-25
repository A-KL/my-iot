using Griffin.Networking.Protocol.Http.Protocol;
using Griffin.Networking.Web.Serialization;

namespace Griffin.Networking.Web.Handlers.WebApi
{
    using System;
    using System.Collections.Generic;
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
        private readonly IDictionary<string, ApiControllerInfo> controllers = new Dictionary<string, ApiControllerInfo>();

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
            this.controllers.Clear();

            var results = ApiControllerInfo.Lookup<ApiController>(this.assembly, new NamingAttributesResolver(), new ContentTypeFactory());

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

        public void Invoke(IRequest request)
        {
            foreach (var route in this.routes)
            {
                IDictionary<string, object> variables;
                
                if (!MatchUriToRoute(request.Uri.LocalPath, route, out variables))
                {
                    continue;
                }

                var controllerName = variables["controller"].ToString();
                variables.Remove("controller");

                if (this.controllers.ContainsKey(controllerName))
                {
                    var controllerInfo = this.controllers[controllerName];

                    controllerInfo.Execute(request, variables);
                }
            }
        }

        private static bool MatchUriToRoute(string localPath, string route, out IDictionary<string, object> variables)
        {
            variables = null;

            var routeUriSegments = route.TrimStart('/').Split('/');
            var uriSegments = localPath.TrimStart('/').Split('/');

            if (routeUriSegments.Length != uriSegments.Length)
            {
                return false;
            }

            variables = new Dictionary<string, object>();

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
