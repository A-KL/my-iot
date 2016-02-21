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

        public void AddMapping(string routePath, RouteHandler handler)
        {
            this.handlers.Add(routePath, handler);
        }

        public override void Dispose()
        {

        }

        public async override void OnRequest(IRequest request)
        {
            foreach (var routeHandler in this.handlers)
            {
                if (request.Uri.LocalPath.Contains(routeHandler.Key))
                {
                    var result = await routeHandler.Value.ExecuteAsync(request);

                    using (result.Body)
                    {
                        this.Send(result);
                    }
                    break;
                }
            }
        }
    }


    //public class WebApi2 : RouteHandler
    //{
    //    public void Register<T>(string path)
    //    {

    //    }

    //    public override Task<IResponse> Request<IResponse>(IRequest request)
    //    {
    //        return null;
    //    }
    //}
}
