using System.Net;
using System.Net.Http;

namespace System.Web.Http
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RouteAttribute : Attribute
    {
        public string Template { get; set; }

        public HttpMethod Method { get; private set; }

        public RouteAttribute(string template, HttpMethod method)
        {
            this.Method = method;
            this.Template = template;
        }
    }
}