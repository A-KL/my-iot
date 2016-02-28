using System.Net.Http;
using System.Reflection;

namespace Microsoft.Iot.Web.Api.Resolver
{
    public interface INamingResolver
    {
        HttpMethod Reslove(MethodInfo method);
    }
}