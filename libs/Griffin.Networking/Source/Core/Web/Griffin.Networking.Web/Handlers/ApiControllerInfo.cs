using System.Threading.Tasks;
using Griffin.Networking.Protocol.Http.Protocol;

namespace Griffin.Networking.Web.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Http;

    public class ApiControllerInfo
    {
        private Type controllerType;

        private IEnumerable<string> routes;


        public ApiControllerInfo(Type controllerType)
        {
            this.controllerType = controllerType;

            this.Name = GetRouteFromControllerName(this.controllerType);
        }

        public string Name
        {
            get; private set;
        }

        public IEnumerable<string> AttributeRoutes
        {
            get
            {
                return (this.routes ?? (this.routes = GetAttributeRoutes(this.controllerType)));
            }
        }

        public static IList<ApiControllerInfo> Lookup<T>(Assembly assembly)
        {
            return (from assemblyType in assembly.GetTypes()
                    where typeof(T).IsAssignableFrom(assemblyType)
                    select new ApiControllerInfo(assemblyType))
                .ToList();
        }

        public void Execute(IRequest request, IDictionary<string, string> variables)
        {
            using (var controller = (ApiController)Activator.CreateInstance(this.controllerType)) // TODO: add DI here
            {
                //controller.Request = request;


            }
        }

        private static IEnumerable<string> GetAttributeRoutes(Type type)
        {
            var routePrefixAttribute = type.GetTypeInfo().GetCustomAttribute<RoutePrefixAttribute>();

            var prefix = routePrefixAttribute == null ? null : routePrefixAttribute.Template;

            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.FlattenHierarchy);

            foreach (var methodInfo in methods)
            {
                var routeAttribute = methodInfo.GetCustomAttribute<RouteAttribute>();

                yield return (prefix == null ? routeAttribute.Template : Combine(prefix, routeAttribute.Template));
            }
        }

        private static string Combine(string baseUri, string relativeUri)
        {
            if (string.IsNullOrEmpty(relativeUri))
            {
                return baseUri.TrimEnd('/');
            }

            return baseUri.TrimEnd('/') + "/" + relativeUri.TrimStart('/');
        }

        private static string GetRouteFromControllerName(Type type)
        {
            string[] exclusions = { "Controller", "ApiController" };

            foreach (var exclude in exclusions.OrderBy(e => e.Length))
            {
                if (type.Name.EndsWith(exclude, StringComparison.OrdinalIgnoreCase))
                {
                    return type.Name.Replace(exclude, string.Empty).ToLower();
                }
            }

            return type.Name;
        }
    }
}