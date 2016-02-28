using System.Net.Http;

namespace System.Web.Http
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpGetAttribute : RouteAttribute
    {
        public HttpGetAttribute(string template)
            : base(template, HttpMethod.Get)
        {
        }
    }
}