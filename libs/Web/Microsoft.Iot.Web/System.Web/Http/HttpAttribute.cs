using System;
using System.Net.Http;

namespace System.Web.Http
{
    public abstract class HttpAttribute : Attribute
    {
        public HttpMethod Method { get; }

        protected HttpAttribute(HttpMethod method)
        {
            this.Method = method;
        }
    }
}
