using System.Net.Http;

namespace System.Web.Http
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPostAttribute : RouteAttribute
    {
        public HttpPostAttribute(string template)
            : base(template, HttpMethod.Post)
        {
        }
    }
}