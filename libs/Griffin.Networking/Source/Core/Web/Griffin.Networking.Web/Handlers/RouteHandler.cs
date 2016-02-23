namespace Griffin.Networking.Web.Handlers
{
    using System.Threading.Tasks;
    using Protocol.Http.Protocol;

    public abstract class RouteHandler
    {
        public abstract Task<IResponse> ExecuteAsync(string localPath, IRequest request);
    }
}
