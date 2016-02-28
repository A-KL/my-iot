namespace Microsoft.Iot.Web.Api
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Griffin.Networking.Protocol.Http.Protocol;
    using Griffin.Networking.Web;
    using Resolver;
    using Serialization;

    public class WebApiListener : RouteListener
    {
        private readonly IDictionary<string, ApiControllerInfo> controllers = new Dictionary<string, ApiControllerInfo>();

        private readonly Assembly assembly;

        private IList<string> routes;

        public override bool IsListeningTo(Uri uri)
        {
            return true;
        }

        public WebApiListener(Assembly assembly, IDictionary<string, string> map = null)
        {
            this.assembly = assembly;

            if (map != null)
            {
                this.routes = new List<string>();

                this.routes.AddRange(map.Values);
            }
            this.Init();
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

        public override Task<IResponse> ExecuteAsync(IRequest request, IDependencyResolver resolver)
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

                    return controllerInfo.Execute(request, variables, resolver);
                }
            }

            return null;
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
