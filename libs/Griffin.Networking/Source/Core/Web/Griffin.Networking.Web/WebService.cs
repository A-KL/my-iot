namespace Griffin.Networking.Web
{
    using System.Collections.Generic;
    using Buffers;
    using Handlers;
    using Handlers.WebApi;
    using Protocol.Http;
    using Protocol.Http.Protocol;

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
                foreach (var route in routeHandler.Routes)
                {
                    if (request.Uri.LocalPath.StartsWith(route))
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
        }

        public override void Dispose()
        {

        }
    }
}
