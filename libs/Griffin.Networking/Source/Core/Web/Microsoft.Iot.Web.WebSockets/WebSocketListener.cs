using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Web.Http;
using Windows.Security.Cryptography.Core;

using Griffin.Networking.Protocol.Http.Protocol;

namespace Microsoft.Iot.Web.WebSockets
{
    public class WebSocketListener : RouteListener
    {
        private const string Guid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

        private const string UpgradeHeader = "Upgrade";
        private const string ConnectionHeader = "Connection";

        private const string WebSocketSecKeyHeader = "Sec-WebSocket-Key";
        private const string WebSocketSecProtocolHeader = "Sec-WebSocket-Protocol";
        private const string WebSocketSecVersionHeader = "Sec-WebSocket-Version";

        private const string WebSocketSecAcceptHeader = "Sec-WebSocket-Accept";

        private readonly IDictionary<IRequest, IResponse> client = new Dictionary<IRequest, IResponse>();

        public override bool IsListeningTo(Uri uri)
        {
            return (uri.Segments[1] == "signalr");
        }

        public override Task<IResponse> ExecuteAsync(IRequest request, IDependencyResolver resolver)
        {
            var badRequest = request.CreateResponse(HttpStatusCode.BadRequest, "Wrong header value");

            if (request.Headers[ConnectionHeader].Value != "Upgrade")
            {
                return Task.FromResult(badRequest);
            }

            if (request.Headers[UpgradeHeader].Value != "websocket")
            {
                return Task.FromResult(badRequest);
            }

            var response = request.CreateResponse(HttpStatusCode.SwitchingProtocols, "Switching Protocols");
            var key = request.Headers[WebSocketSecKeyHeader].Value;
            var hash = AcceptKey(ref key);

            response.AddHeader(ConnectionHeader, "Upgrade");
            response.AddHeader(UpgradeHeader, "websocket");
            //response.AddHeader("Access-Control-Allow-Origin", "*");
            //response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS, PUT, DELETE");
            response.AddHeader(WebSocketSecAcceptHeader, hash);

            client.Add(request, response);

            return Task.FromResult(response);
        }

        private string AcceptKey(ref string key)
        {
            var longKey = key + Guid;

            var sha1 = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");

            var hashBytes = sha1.CreateHash(System.Text.Encoding.ASCII.GetBytes(longKey).AsBuffer()).GetValueAndReset().ToArray();

            return Convert.ToBase64String(hashBytes);
        }
    }
}
