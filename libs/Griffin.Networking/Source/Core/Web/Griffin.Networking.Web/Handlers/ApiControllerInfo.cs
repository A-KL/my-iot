using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Griffin.Networking.Web.Handlers
{
    public class ApiControllerInfo
    {
        private Type controllerType;

        private IList<string> resources;

        public ApiControllerInfo(Type controllerType)
        {
            this.controllerType = controllerType;

            this.resources = this.Resolve(this.controllerType);
        }

        public static IList<ApiControllerInfo> Lookup<T>(Assembly assembly)
        {
            return (from assemblyType in assembly.GetTypes()
                    where typeof(T).IsAssignableFrom(assemblyType)
                    select new ApiControllerInfo(assemblyType))
                .ToList();
        }

        private IList<string> Resolve(Type type)
        {
            var prefix = RouteHelper.ResolvePrefix(type);

            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);

            foreach (var methodInfo in methods)
            {
                var info = RouteHelper.Resolve(methodInfo);
            }

            return null;
        }
    }
}