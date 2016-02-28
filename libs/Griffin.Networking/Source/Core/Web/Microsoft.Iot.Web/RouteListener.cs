using System;
using System.Threading.Tasks;
using System.Web.Http;
using Griffin.Networking.Protocol.Http.Protocol;

namespace Microsoft.Iot.Web
{
    public abstract class RouteListener
    {
        public abstract bool IsListeningTo(Uri uri);

        public abstract Task<IResponse> ExecuteAsync(IRequest request, IDependencyResolver resolver);
    }
}
