using System;
using System.Net.Http;
using System.Web.Http;

namespace Griffin.Networking.Web.System.Web.Http
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPutAttribute : HttpAttribute
    {
        public HttpPutAttribute()
            : base(HttpMethod.Put)
        {
        }
    }
}
