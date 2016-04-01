using System;
using System.Threading.Tasks;
using System.Web.Http;
using Griffin.Net.Protocols.Http;

namespace Microsoft.Iot.Web
{
    public abstract class RouteListener
    {
        public abstract bool IsListeningTo(Uri uri);

        public abstract Task<IHttpResponse> ExecuteAsync(IHttpRequest request, IDependencyResolver resolver);
    }
}
