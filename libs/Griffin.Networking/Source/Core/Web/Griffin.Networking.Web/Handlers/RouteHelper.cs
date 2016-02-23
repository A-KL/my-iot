using System;
using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace Griffin.Networking.Web.Handlers
{
    public class RouteHelper
    {
        public static string ResolvePrefix(Type type)
        {
            var routeFromName = GetRouteFromControllerName(type);

            var routeAttribute = type.GetTypeInfo().GetCustomAttribute<RoutePrefixAttribute>();
            if (routeAttribute == null)
            {
                return routeFromName;
            }

            if (TemplateRouteBuilder.HasParamets(routeAttribute.Template))
            {
                var builder = new TemplateRouteBuilder(routeAttribute.Template);

                if (builder.Parametrs.ContainsKey("controller"))
                {
                    builder.Parametrs["controller"] = routeFromName;
                }

                return builder.ToString();
            }

            return routeAttribute.Template;
        }

        public static string Resolve(MethodInfo method)
        {
            var routeAttribute = method.GetCustomAttribute<RouteAttribute>();

            //  var route = GetRouteFromControllerName(type);

            if (routeAttribute == null)
            {
                // route = GetRouteFromControllerName(type);
            }
            else
            {

            }

            return null;
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