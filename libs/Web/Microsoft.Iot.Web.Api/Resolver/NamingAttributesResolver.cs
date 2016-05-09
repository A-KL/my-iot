using System.Net.Http;
using System.Reflection;
using System.Web.Http;

namespace Microsoft.Iot.Web.Api.Resolver
{
    public class NamingAttributesResolver : INamingResolver
    {
        public HttpMethod Reslove(MethodInfo method)
        {
            var attribute = method.GetCustomAttribute<HttpAttribute>();

            return attribute?.Method;
        }
    }
}