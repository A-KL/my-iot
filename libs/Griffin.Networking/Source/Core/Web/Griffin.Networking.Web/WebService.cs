using System.Net.Http;
using Griffin.Networking.Protocol.Http.Implementation;

namespace Griffin.Networking.Web
{
    using System.Collections.Generic;
    using Buffers;
    using Handlers;
    using Protocol.Http;
    using Protocol.Http.Protocol;

    public class WebServiceSettings
    {
        private readonly IDictionary<string, RouteHandler> handlers = new Dictionary<string, RouteHandler>();

        public IDictionary<string, RouteHandler> Handlers
        {
            get { return this.handlers; }
        }
    }

    public class WebService : HttpService
    {
        private readonly static BufferSliceStack Stack = new BufferSliceStack(50, 32000);

        private IDictionary<string, RouteHandler> handlers = new Dictionary<string, RouteHandler>();

        public WebService(WebServiceSettings settings)
            : base(Stack)
        {
            this.handlers = settings.Handlers;
        }

        public async override void OnRequest(IRequest request)
        {
            foreach (var routeHandler in this.handlers)
            {
                if (request.Uri.LocalPath.StartsWith(routeHandler.Key))
                {
                    var result = await routeHandler.Value.ExecuteAsync(routeHandler.Key, request);

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
