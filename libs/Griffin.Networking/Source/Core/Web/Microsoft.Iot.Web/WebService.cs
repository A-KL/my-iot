using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Iot.Web;

namespace Griffin.Networking.Web
{
    using Buffers;
    using Protocol.Http;
    using Protocol.Http.Protocol;

    public class WebService : HttpService
    {
        private static readonly BufferSliceStack Stack = new BufferSliceStack(50, 32000);

        private readonly IList<RouteListener> handlers;

        private readonly HttpConfiguration settings;

        public WebService(HttpConfiguration settings)
            : base(Stack)
        {
            this.settings = settings;

            this.handlers = this.settings.Listeners;
        }

        public override async void OnRequest(IRequest request)
        {
            var localPath = request.Uri.LocalPath.TrimEnd('/');

            if (string.IsNullOrEmpty(localPath) && !string.IsNullOrEmpty(settings.DefaultPath))
            {
                request.Uri = new Uri(request.Uri.AbsoluteUri.TrimEnd('/') + "/" + settings.DefaultPath);
            }

            try
            {

                foreach (var routeHandler in this.handlers)
                {
                    if (routeHandler.IsListeningTo(request.Uri))
                    {
                        var result = await routeHandler.ExecuteAsync(request, settings.DependencyResolver);                        
                        this.Send(result);
                        break;
                    }
                }
            }
            catch (Exception error)
            {
                var response = request.CreateResponse(HttpStatusCode.InternalServerError, error.Message);

                this.Send(response);                
            }
        }

        public override void Dispose()
        {

        }
    }
}
