using System.Net;

namespace System.Web.Http
{
    public class HttpResponseMessage : IDisposable
    {
        public HttpResponseMessage(HttpStatusCode statusCode)
        {

        }

        public HttpRequestMessage RequestMessage { get; set; }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

}
