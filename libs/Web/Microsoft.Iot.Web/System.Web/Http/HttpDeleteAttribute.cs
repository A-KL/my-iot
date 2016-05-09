using System;
using System.Net.Http;
using System.Web.Http;

namespace Griffin.Networking.Web.System.Web.Http
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpDeleteAttribute : HttpAttribute
    {
        public HttpDeleteAttribute()
            : base(HttpMethod.Delete)
        {
        }
    }
}
