using System;
using System.Net.Http;
using System.Web.Http;

namespace Griffin.Networking.Web.System.Web.Http
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPutAttribute : RouteAttribute
    {
        public HttpPutAttribute(string template)
            : base(template, HttpMethod.Put)
        {
        }
    }
}
