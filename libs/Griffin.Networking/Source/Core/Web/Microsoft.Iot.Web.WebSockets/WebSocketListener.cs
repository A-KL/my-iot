using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Windows.Foundation.Metadata;

namespace Microsoft.Iot.Web.WebSockets
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Griffin.Networking.Protocol.Http.Protocol;
    using Windows.Security.Cryptography;
    using Windows.Security.Cryptography.Core;
    using Windows.Storage.Streams;

    public abstract class BaseHub : IHub
    {
        private readonly IList<Stream> connections = new List<Stream>();

        protected BaseHub(string path)
        {
            this.Path = path;
        }

        public string Path { get; private set; }

        public bool OnConnectionRequest(IRequest request)
        {
            return true;
        }
        
        public void Dispose()
        {
            throw new NotImplementedException();
        }        
    }

    public interface IHub : IDisposable
    {
        string Path { get; }

        bool OnConnectionRequest(IRequest request);
    }

    public class WebSocketListener : RouteListener
    {
        private const string Guid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

        private const string UpgradeHeader = "Upgrade";
        private const string ConnectionHeader = "Connection";

        private const string WebSocketSecKeyHeader = "Sec-WebSocket-Key";
        private const string WebSocketSecProtocolHeader = "Sec-WebSocket-Protocol";
        private const string WebSocketSecVersionHeader = "Sec-WebSocket-Version";

        private const string WebSocketSecAcceptHeader = "Sec-WebSocket-Accept";

        public IDictionary<string, IHub> Hubs { get; } = new Dictionary<string, IHub>();

        public override bool IsListeningTo(Uri uri)
        {
            return this.Hubs.ContainsKey(uri.LocalPath);
        }

        public override Task<IResponse> ExecuteAsync(IRequest request, IDependencyResolver resolver)
        {
            var badRequest = request.CreateResponse(HttpStatusCode.BadRequest, "Wrong header value");

            if (!request.IsWebSocketRequest)
            {
                return Task.FromResult(badRequest);
            }

            var hub = this.Hubs[request.Uri.LocalPath];

            if (hub.OnConnectionRequest(request))
            {
                var response = request.CreateResponse(HttpStatusCode.SwitchingProtocols, "Switching Protocols");
                var key = request.Headers[WebSocketSecKeyHeader].Value;
                var hash = this.AcceptKey(ref key);

                response.AddHeader(ConnectionHeader, "Upgrade");
                response.AddHeader(UpgradeHeader, "websocket");
                response.AddHeader(WebSocketSecAcceptHeader, hash);

                return Task.FromResult(response);
            }

            return Task.FromResult(badRequest);

        }

        private string AcceptKey(ref string key)
        {
            var longKey = key + Guid;

            var hash = AlgorithmHelper.ComputeHash(longKey, HashAlgorithmNames.Sha1);

            return Convert.ToBase64String(hash.ToArray());
        }

        private string DecodeWebSocketMessage(byte[] data)
        {
            var info = data[0];
            var size = data[1] - 128;

            var decoded = new byte[size];
            var encoded = new byte[size];

            Array.Copy(data, 2 + 4, encoded, 0, size);

            var key = new byte[4];

            Array.Copy(data, 2, key, 0, 4);

            for (var i = 0; i < encoded.Length; i++)
            {
                decoded[i] = (byte)(encoded[i] ^ key[i % 4]);
            }

            return Encoding.UTF8.GetString(decoded);
        }
    }

    internal class AlgorithmHelper
    {
        /// <summary>
        /// Computes hash algorithm for the source string
        /// </summary>
        /// <param name="source">Source string to compute hash from</param>
        /// <param name="algorithm">HashAlgorithmNames.Sha1</param>
        /// <returns>hash from the source string</returns>
        public static IBuffer ComputeHash(string source, string algorithm)
        {
            var sha1 = HashAlgorithmProvider.OpenAlgorithm(algorithm);
            var bytes = Encoding.UTF8.GetBytes(source);
            var bytesBuffer = CryptographicBuffer.CreateFromByteArray(bytes);
            var hashBuffer = sha1.HashData(bytesBuffer);

            return hashBuffer;
        }
    }
}
