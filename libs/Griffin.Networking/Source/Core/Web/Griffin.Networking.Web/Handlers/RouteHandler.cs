namespace Griffin.Networking.Web.Handlers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Protocol.Http.Protocol;

    public abstract class RouteHandler
    {
        public abstract IEnumerable<string> Routes { get; }

        public abstract Task<IResponse> ExecuteAsync(IRequest request);
    }
}
