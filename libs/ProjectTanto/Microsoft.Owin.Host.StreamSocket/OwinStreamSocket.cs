using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace Microsoft.Owin.Host.StreamSocket
{
    using AppFunc = Func<IDictionary<string, object>, Task>;
    using LoggerFactoryFunc = Func<string, Func<TraceEventType, int, object, Exception, Func<object, Exception, string>, bool>>;
    using LoggerFunc = Func<TraceEventType, int, object, Exception, Func<object, Exception, string>, bool>;

    public sealed class OwinStreamSocket : IDisposable
    {
        private Action startNextRequestAsync;
        private Action<Task> startNextRequestError;

        private StreamSocketListener listener;
        private IList<string> basePaths;
        private AppFunc appFunc;
       // private DisconnectHandler _disconnectHandler;
        private IDictionary<string, object> capabilities;

        public void Dispose()
        {
        }

        /// <summary>
        /// The HttpListener instance wrapped by this wrapper.
        /// </summary>
        public StreamSocketListener Listener
        {
            get { return this.listener; }
        }

        /// <summary>
        /// Starts the listener and request processing threads.
        /// </summary>
        internal void Start(
            StreamSocketListener listener,
            AppFunc appFunc,
            IList<IDictionary<string, object>> addresses,
            IDictionary<string, object> capabilities,
            LoggerFactoryFunc loggerFactory)
        {
            Contract.Assert(this.appFunc == null); // Start should only be called once
            Contract.Assert(listener != null);
            Contract.Assert(appFunc != null);
            Contract.Assert(addresses != null);

            this.listener = listener;
            this.appFunc = appFunc;
            //_logger = LogHelper.CreateLogger(loggerFactory, typeof(OwinHttpListener));

            basePaths = new List<string>();

            foreach (var address in addresses)
            {
                // build url from parts
                string scheme = address.Get<string>("scheme");// ?? Uri.UriSchemeHttp;
                string host = address.Get<string>("host") ?? "localhost";
                string port = address.Get<string>("port") ?? "5000";
                string path = address.Get<string>("path") ?? string.Empty;

                // if port is present, add delimiter to value before concatenation
                if (!string.IsNullOrWhiteSpace(port))
                {
                    port = ":" + port;
                }

                // Assume http(s)://+:9090/BasePath/, including the first path slash.  May be empty. Must end with a slash.
                if (!path.EndsWith("/", StringComparison.Ordinal))
                {
                    // Http.Sys requires that the URL end in a slash
                    path += "/";
                }
                basePaths.Add(path);

                // add a server for each url
                var url = scheme + "://" + host + port + path;

               // this.listener.Prefixes.Add(url);
            }

            this.capabilities = capabilities;

            //if (!this.listener.IsListening)
            //{
            //    this.listener.Start();
            //}

            //SetRequestQueueLimit();

            //_disconnectHandler = new DisconnectHandler(this.listener, _logger);

            // OffloadStartNextRequest();
        }
    }
}
