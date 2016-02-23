namespace Griffin.Networking.Web
{
    using System.Collections.Generic;
    using Buffers;
    using Handlers;
    using Protocol.Http;
    using Protocol.Http.Protocol;    
    using Griffin.Networking.Protocol.Http.Implementation;

    public class WebServiceSettings
    {
        private readonly IList<RouteHandler> handlers = new List<RouteHandler>();

        public IList<RouteHandler> Handlers
        {
            get { return this.handlers; }
        }
    }

    public class WebService : HttpService
    {
        private readonly static BufferSliceStack Stack = new BufferSliceStack(50, 32000);

        private readonly IList<RouteHandler> handlers = new List<RouteHandler>();

        public WebService(WebServiceSettings settings)
            : base(Stack)
        {
            this.handlers = settings.Handlers;
        }

        public async override void OnRequest(IRequest request)
        {
            foreach (var routeHandler in this.handlers)
            {
                if (request.Uri.LocalPath.StartsWith(routeHandler.Route))
                {
                    var result = await routeHandler.ExecuteAsync(request);

                    // using (result.Body)
                    // {
                    this.Send(result);
                    // }
                    break;
                }
            }
        }

        public override void Dispose()
        {

        }
    }
}
