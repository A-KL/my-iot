namespace Griffin.Networking.Web.Handlers.WebApi
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Web.Http;

    public class RouteAttributeHelper
    {
        public static string ResolvePrefix(Type type)
        {
            var routeAttribute = type.GetTypeInfo().GetCustomAttribute<RoutePrefixAttribute>();
            if (routeAttribute == null)
            {
                return null;
            }

            if (TemplateRouteBuilder.HasParamets(routeAttribute.Template))
            {
                var builder = new TemplateRouteBuilder(routeAttribute.Template);

                if (builder.Parametrs.ContainsKey("controller"))
                {
                    builder.Parametrs["controller"] = GetRouteFromControllerName(type);
                }

                return builder.ToString();
            }

            return routeAttribute.Template;
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