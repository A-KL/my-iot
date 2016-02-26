using Griffin.Networking.Web.Listeners.WebApi;

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
        private readonly IList<RouteListener> listeners = new List<RouteListener>();

        public IList<RouteListener> Listeners
        {
            get { return this.listeners; }
        }
    }

    public class WebService : HttpService
    {
        private readonly static BufferSliceStack Stack = new BufferSliceStack(50, 32000);

        private readonly IList<RouteListener> handlers = new List<RouteListener>();

        public WebService(WebServiceSettings settings)
            : base(Stack)
        {
            this.handlers = settings.Listeners;
        }

        public async override void OnRequest(IRequest request)
        {
            foreach (var routeHandler in this.handlers)
            {
                // if (request.Uri.LocalPath.StartsWith(route))
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
