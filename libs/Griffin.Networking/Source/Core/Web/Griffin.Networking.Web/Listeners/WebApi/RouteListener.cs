using System;
using System.Threading.Tasks;

using Griffin.Networking.Protocol.Http.Protocol;

namespace Griffin.Networking.Web.Listeners.WebApi
{
    public abstract class RouteListener
    {
        public abstract bool IsListeningTo(Uri uri);

        public abstract Task<IResponse> ExecuteAsync(IRequest request);
    }
}
