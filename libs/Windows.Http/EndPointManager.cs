namespace Windows.Http
{
    using global::System;
    using global::System.Collections;
    using global::System.Collections.Generic;
    using global::System.Net;
    using global::System.Threading.Tasks;

    public static class EndPointManager
    {
        private static readonly Dictionary<IPAddress, Dictionary<int, EndPointListener>> IpToEndpoints = new Dictionary<IPAddress, Dictionary<int, EndPointListener>>();

        public static async Task AddListener(HttpListener listener)
        {
            var added = new ArrayList();
            try
            {
                foreach (var prefix in listener.Prefixes)
                {
                    await AddPrefixInternal(prefix, listener);
                    added.Add(prefix);
                }
            }
            catch
            {
                foreach (string prefix in added)
                {
                   await RemovePrefix(prefix, listener);
                }

                throw;
            }
        }

        public static async Task RemoveListener(HttpListener listener)
        {
            foreach (var prefix in listener.Prefixes)
            {
                await RemovePrefixInternal(prefix, listener);
            }
        }


        public static Task AddPrefix(string prefix, HttpListener listener)
        {
            return AddPrefixInternal(prefix, listener);            
        }

        public static Task RemovePrefix(string prefix, HttpListener listener)
        {
            return RemovePrefixInternal(prefix, listener);            
        }


        public static void RemoveEndPoint(EndPointListener endPointListener, IPEndPoint endPoint)
        {
            var p = IpToEndpoints[endPoint.Address];
            p.Remove(endPoint.Port);

            if (p.Count == 0)
            {
                IpToEndpoints.Remove(endPoint.Address);
            }

            endPointListener.Close();            
        }
        

        private static async Task AddPrefixInternal(string prefix, HttpListener listener)
        {
            var iistenerPrefix = new ListenerPrefix(prefix);

            if (iistenerPrefix.Path.IndexOf('%') != -1)
            {
                throw new HttpListenerException(400, "Invalid path.");
            }

            if (iistenerPrefix.Path.IndexOf("//", StringComparison.Ordinal) != -1)
            {
                throw new HttpListenerException(400, "Invalid path.");
            }

            // listens on all the interfaces if host name cannot be parsed by IPAddress.
            var epl = await GetEndPointListener(iistenerPrefix.Host, iistenerPrefix.Port);

            epl.AddPrefix(iistenerPrefix, listener);
        }

        private static async Task RemovePrefixInternal(string prefix, HttpListener listener)
        {
            var lp = new ListenerPrefix(prefix);

            if (lp.Path.IndexOf('%') != -1)
            {
                return;
            }

            if (lp.Path.IndexOf("//", StringComparison.Ordinal) != -1)
            {
                return;
            }

            var epl = await GetEndPointListener(lp.Host, lp.Port);
            epl.RemovePrefix(lp, listener);
        }

        private static async Task<EndPointListener> GetEndPointListener(string host, int port)
        {
            IPAddress addr;

            if (!IPAddress.TryParse(host, out addr))
            {
                addr = IPAddress.Any;
            }

            Dictionary<int, EndPointListener> portToListener;

            if (IpToEndpoints.ContainsKey(addr))
            {
                portToListener = IpToEndpoints[addr];
            }
            else
            {
                portToListener = new Dictionary<int, EndPointListener>();
                IpToEndpoints[addr] = portToListener;
            }

            EndPointListener epl;

            if (portToListener.ContainsKey(port))
            {
                epl = portToListener[port];
            }
            else
            {
                epl = new EndPointListener(addr, port);
                portToListener[port] = epl;
                await epl.Bind();
            }

            return epl;
        }
    }
}