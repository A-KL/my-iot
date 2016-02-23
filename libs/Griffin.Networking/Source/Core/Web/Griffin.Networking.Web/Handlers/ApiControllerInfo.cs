using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace Griffin.Networking.Web.Handlers
{
    public class ApiControllerMethodInfo
    {
        public ApiControllerMethodInfo(MethodInfo info)
        {

        }
    }

    public class ApiControllerInfo
    {
        private Type controllerType;

        public IEnumerable<string> AttributeRoutes
        {
            get
            {
                return GetAttributeRoutes(this.controllerType);
            }
        }

        public ApiControllerInfo(Type controllerType)
        {
            this.controllerType = controllerType;

            //this.resources = Resolve(this.controllerType);
        }

        public static IList<ApiControllerInfo> Lookup<T>(Assembly assembly)
        {
            return (from assemblyType in assembly.GetTypes()
                    where typeof(T).IsAssignableFrom(assemblyType)
                    select new ApiControllerInfo(assemblyType))
                .ToList();
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
    }
}