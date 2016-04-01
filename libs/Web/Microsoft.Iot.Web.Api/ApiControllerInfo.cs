using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Griffin.Net.Protocols.Http;
using Microsoft.Iot.Web.Api.Resolver;
using Microsoft.Iot.Web.Serialization;

namespace Microsoft.Iot.Web.Api
{
    public class ApiControllerInfo
    {
        private readonly Type controllerType;

        private readonly IList<string> routes = new List<string>();

        private readonly IDictionary<MethodInfo, HttpMethod> methodsMap = new Dictionary<MethodInfo, HttpMethod>();

        private readonly INamingResolver resolver;

        private readonly ISerializationFactory factory;


        public ApiControllerInfo(Type controllerType, INamingResolver resolver, ISerializationFactory factory)
        {
            this.resolver = resolver;

            this.factory = factory;

            this.controllerType = controllerType;

            this.Name = ResolveControllerName(this.controllerType);

            this.PrepareMethodsInfo();

            this.PrepareRoutesInfo();
        }

        public string Name
        {
            get; private set;
        }

        public IEnumerable<string> AttributeRoutes
        {
            get { return this.routes; }
        }

        public static IList<ApiControllerInfo> Lookup<T>(Assembly assembly, INamingResolver resolver, ISerializationFactory factory)
        {
            return (from assemblyType in assembly.GetTypes()
                    where typeof(T).IsAssignableFrom(assemblyType)
                    select new ApiControllerInfo(assemblyType, resolver, factory))
                .ToList();
        }


        public async Task<IHttpResponse> Execute(IHttpRequest request, IDictionary<string, object> variables, IDependencyResolver resolver)
        {
            using (var controller = CreateInstance<ApiController>(this.controllerType, resolver))
            {
                foreach (var controllerMethod in this.methodsMap)
                {
                    if (request.HttpMethod != controllerMethod.Value.Method)
                    {
                        continue;
                    }

                    var parameters = controllerMethod.Key.GetParameters();

                    if (!IsCompatible(parameters, variables.Keys))
                    {
                        continue;
                    }

                    var serializer = this.factory.Create(request);

                    // Do we need to create a [FromBody] parameter?
                    if (parameters.Length == variables.Count + 1)
                    {
                        var bodyParametr = parameters.Last();

                        variables.Add(bodyParametr.Name, serializer.Deserialize(request.Body, bodyParametr.ParameterType));
                    }

                    variables = ResolveTypes(variables, controllerMethod.Key);

                    controller.Request = request.ToHttpRequestMessage();

                    var returnResult = controllerMethod.Key.Invoke(controller, variables.Values.ToArray());

                    var returnParameter = controllerMethod.Key.ReturnParameter;

                    var response = request.CreateResponse();

                    response.StatusCode = (int)HttpStatusCode.OK;

                    if (returnParameter.ParameterType.IsAssignableFrom(typeof(HttpResponseMessage)))
                    {
                        var httpResponseMessage = (HttpResponseMessage)returnResult;

                        foreach (var header in httpResponseMessage.Headers)
                        {
                            response.AddHeader(header.Key, header.Value.FirstOrDefault());
                        }

                        foreach (var header in httpResponseMessage.Content.Headers)
                        {
                            response.AddHeader(header.Key, header.Value.FirstOrDefault());
                        }

                        return response;
                    }

                    if (returnParameter.ParameterType == typeof(void))
                    {
                        return response;
                    }

                    if (returnParameter.ParameterType == typeof(Task))
                    {
                        await (Task)returnResult;

                        return response;
                    }

                    if (returnParameter.ParameterType.IsAssignableFrom(typeof(Task)))
                    {
                        returnResult = await (dynamic)returnResult;
                    }

                    var stream = new MemoryStream();

                    var body = serializer.Serialize(returnResult);

                    var bin = Encoding.UTF8.GetBytes(body);

                    await stream.WriteAsync(bin, 0, bin.Length);
                    stream.Position = 0;

                    response.Body = stream;
                    response.ContentType = request.ContentType ?? serializer.ContentType;
                    response.ContentLength = bin.Length;

                    return response;
                }
            }

            return null;
        }

        private static IDictionary<string, object> ResolveTypes(IDictionary<string, object> variables, MethodInfo methodInfo)
        {
            var result = new Dictionary<string, object>();

            foreach (var parameterInfo in methodInfo.GetParameters())
            {
                object parameterValue = variables[parameterInfo.Name];

                var targetType = parameterInfo.ParameterType;

                var sourceType = parameterValue.GetType();

                if (!targetType.IsAssignableFrom(sourceType))
                {
                    parameterValue = Convert.ChangeType(parameterValue, targetType);
                }

                result.Add(parameterInfo.Name, parameterValue);
            }

            return result;
        }

        private static T CreateInstance<T>(Type type, IDependencyResolver resolver)
        {
            if (resolver == null)
            {
                return (T)Activator.CreateInstance(type);
            }

            var constructorsInfos =
                type.GetConstructors(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);

            var constructorInfo = constructorsInfos.OrderByDescending(c => c.GetParameters()).First();

            var parameters = constructorInfo.GetParameters();
            if (parameters.Length == 0)
            {
                return (T)Activator.CreateInstance(type);
            }

            var paramsToCreate = new List<object>();

            foreach (var parameterInfo in parameters)
            {
                paramsToCreate.Add(resolver.GetService(parameterInfo.ParameterType));
            }

            return (T)Activator.CreateInstance(type, paramsToCreate.ToArray());
        }

        private void PrepareMethodsInfo()
        {
            this.methodsMap.Clear();

            var methods = this.controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.FlattenHierarchy);

            foreach (var methodInfo in methods)
            {
                this.methodsMap.Add(methodInfo, this.resolver.Reslove(methodInfo));
            }
        }

        private void PrepareRoutesInfo()
        {
            this.routes.Clear();

            var routePrefixAttribute = this.controllerType.GetTypeInfo().GetCustomAttribute<RoutePrefixAttribute>();

            var prefix = routePrefixAttribute == null ? null : routePrefixAttribute.Template;

            foreach (var methodInfoPair in this.methodsMap)
            {
                var routeAttribute = methodInfoPair.Key.GetCustomAttribute<RouteAttribute>();

                this.routes.Add(prefix == null ? routeAttribute.Template : Combine(prefix, routeAttribute.Template));
            }
        }

        private static bool IsCompatible(ICollection<ParameterInfo> paramInfos, ICollection<string> paramNames)
        {
            if (paramInfos.Count < paramNames.Count)
            {
                return false;
            }

            foreach (var paramName in paramNames)
            {
                if (!paramInfos.Select(p => p.Name).Contains(paramName))
                {
                    return false;
                }
            }

            return true;
        }

        private static string ResolveControllerName(Type type)
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