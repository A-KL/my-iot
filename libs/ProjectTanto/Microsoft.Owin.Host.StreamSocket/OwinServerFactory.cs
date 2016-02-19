using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace Microsoft.Owin.Host.StreamSocket
{
    using AddressList = IList<IDictionary<string, object>>;
    using AppFunc = Func<IDictionary<string, object>, Task>;
    using CapabilitiesDictionary = IDictionary<string, object>;
    using LoggerFactoryFunc = Func<string, Func<TraceEventType, int, object, Exception, Func<object, Exception, string>, bool>>;
    using LoggerFunc = Func<TraceEventType, int, object, Exception, Func<object, Exception, string>, bool>;

    /// <summary>
    /// Implements the Katana setup pattern for the OwinHttpListener server.
    /// </summary>
    public static class OwinServerFactory
    {
        /// <summary>
        /// Advertise the capabilities of the server.
        /// </summary>
        /// <param name="properties"></param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed by server later.")]
        public static void Initialize(IDictionary<string, object> properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }

            properties[Constants.VersionKey] = Constants.OwinVersion;

            var capabilities =
                properties.Get<CapabilitiesDictionary>(Constants.ServerCapabilitiesKey)
                    ?? new Dictionary<string, object>();

            properties[Constants.ServerCapabilitiesKey] = capabilities;

            //DetectWebSocketSupport(properties);

            // Let users set advanced configurations directly.
            var wrapper = new OwinStreamSocket();

            properties[typeof(OwinStreamSocket).FullName] = wrapper;
            properties[typeof(StreamSocketListener).FullName] = wrapper.Listener;
        }

        /// <summary>
        /// Creates an OwinHttpListener and starts listening on the given URL.
        /// </summary>
        /// <param name="app">The application entry point.</param>
        /// <param name="properties">The addresses to listen on.</param>
        /// <returns>The OwinHttpListener.  Invoke Dispose to shut down.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed by caller")]
        public static IDisposable Create(AppFunc app, IDictionary<string, object> properties)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }

            // Retrieve the instances created in Initialize
            var wrapper = properties.Get<OwinStreamSocket>(typeof(OwinStreamSocket).FullName)
                ?? new OwinStreamSocket();

            var listener = properties.Get<StreamSocketListener>(typeof(StreamSocketListener).FullName)
                ?? new StreamSocketListener();

            var addresses = properties.Get<AddressList>(Constants.HostAddressesKey)
                ?? new List<IDictionary<string, object>>();

            var capabilities =
                properties.Get<CapabilitiesDictionary>(Constants.ServerCapabilitiesKey)
                    ?? new Dictionary<string, object>();

            var loggerFactory = properties.Get<LoggerFactoryFunc>(Constants.ServerLoggerFactoryKey);

            wrapper.Start(listener, app, addresses, capabilities, loggerFactory);

            return wrapper;
        }
    }
}
