namespace Griffin.Networking.Web.Handlers
{
    using System.Threading.Tasks;
    using Protocol.Http.Protocol;

    public abstract class RouteHandler
    {
        public abstract string Route { get; }

        public abstract Task<IResponse> ExecuteAsync(IRequest request);
    }
}
