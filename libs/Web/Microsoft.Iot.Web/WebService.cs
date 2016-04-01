using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

using Griffin.Net.Channels;
using Griffin.Net.Protocols.Http;

using Microsoft.Iot.Web;

namespace Griffin.Networking.Web
{

    public class WebService : HttpListener
    {
        private readonly IList<RouteListener> handlers;

        private readonly HttpConfiguration settings;

        public WebService(HttpConfiguration settings)
        {
            this.settings = settings;

            this.MessageReceived = this.OnMessage;

            this.handlers = this.settings.Listeners;
        }

        private async void OnMessage(ITcpChannel channel, object message)
        {
            var request = (IHttpRequest)message;

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
                        channel.Send(result);
                        break;
                    }
                }
            }
            catch (Exception error)
            {
                var response = request.CreateResponse(HttpStatusCode.InternalServerError, error.Message);

                channel.Send(response);
            }

            if (request.HttpVersion == "HTTP/1.0")
            {
                channel.Close();
            }
        }
    }
}
