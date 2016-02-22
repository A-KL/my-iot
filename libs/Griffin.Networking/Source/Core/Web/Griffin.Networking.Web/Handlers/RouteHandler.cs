namespace Griffin.Networking.Web.Handlers
{
    using System;
    using System.Threading.Tasks;
    using Protocol.Http.Protocol;

    public abstract class RouteHandler : RequestHandler
    {
        public abstract Task<IResponse> ExecuteAsync(string localPath, IRequest request);
    }
}
