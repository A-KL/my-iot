using System;
using System.Net.Http;
using System.Web.Http;

namespace Griffin.Networking.Web.System.Web.Http
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpDeleteAttribute : RouteAttribute
    {
        public HttpDeleteAttribute(string template)
            : base(template, HttpMethod.Delete)
        {
        }
    }
}
