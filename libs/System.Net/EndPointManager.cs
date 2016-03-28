using System.Collections.Generic;

namespace System.Net
{
    using System.Collections;

    public static class EndPointManager
    {
        private static readonly Dictionary<IPAddress, Dictionary<int, EndPointListener>> IpToEndpoints = new Dictionary<IPAddress, Dictionary<int, EndPointListener>>();

        public static void AddListener(HttpListener listener)
        {
            var added = new ArrayList();
            try
            {
                lock (IpToEndpoints)
                {
                    foreach (var prefix in listener.Prefixes)
                    {
                        AddPrefixInternal(prefix, listener);
                        added.Add(prefix);
                    }
                }
            }
            catch
            {
                foreach (string prefix in added)
                {
                    RemovePrefix(prefix, listener);
                }

                throw;
            }
        }

        public static void AddPrefix(string prefix, HttpListener listener)
        {
            lock (IpToEndpoints)
            {
                AddPrefixInternal(prefix, listener);
            }
        }

        public static void RemoveEndPoint(EndPointListener epl, IPEndPoint ep)
        {
            lock (IpToEndpoints)
            {
                // Dictionary<int, EndPointListener> p
                var p = IpToEndpoints[ep.Address];
                p.Remove(ep.Port);
                if (p.Count == 0)
                {
                    IpToEndpoints.Remove(ep.Address);
                }
                epl.Close();
            }
        }

        public static void RemoveListener(HttpListener listener)
        {
            lock (IpToEndpoints)
            {
                foreach (string prefix in listener.Prefixes)
                {
                    RemovePrefixInternal(prefix, listener);
                }
            }
        }

        public static void RemovePrefix(string prefix, HttpListener listener)
        {
            lock (IpToEndpoints)
            {
                RemovePrefixInternal(prefix, listener);
            }
        }

        private static void AddPrefixInternal(string p, HttpListener listener)
        {
            ListenerPrefix lp = new ListenerPrefix(p);
            if (lp.Path.IndexOf('%') != -1)
                throw new HttpListenerException(400, "Invalid path.");

            if (lp.Path.IndexOf("//", StringComparison.Ordinal) != -1) // TODO: Code?
                throw new HttpListenerException(400, "Invalid path.");

            // listens on all the interfaces if host name cannot be parsed by IPAddress.
            EndPointListener epl = GetEndPointListener(lp.Host, lp.Port, listener, lp.Secure);
            epl.AddPrefix(lp, listener);
        }

        private static EndPointListener GetEndPointListener(string host, int port, HttpListener listener)
        {
            IPAddress addr;
            if (IPAddress.TryParse(host, out addr) == false)
            {
                addr = IPAddress.Any;
            }

            Dictionary<int, EndPointListener> p;

            if (IpToEndpoints.ContainsKey(addr))
            {
                p = IpToEndpoints[addr];
            }
            else
            {
                p = new Dictionary<int, EndPointListener>();
                IpToEndpoints[addr] = p;
            }

            EndPointListener epl;
            if (p.ContainsKey(port))
            {
                epl = p[port];
            }
            else
            {
                epl = new EndPointListener(addr, port);
                p[port] = epl;
            }

            return epl;
        }



        static void RemovePrefixInternal(string prefix, HttpListener listener)
        {
            ListenerPrefix lp = new ListenerPrefix(prefix);
            if (lp.Path.IndexOf('%') != -1)
                return;

            if (lp.Path.IndexOf("//", StringComparison.Ordinal) != -1)
                return;

            EndPointListener epl = GetEPListener(lp.Host, lp.Port, listener, lp.Secure);
            epl.RemovePrefix(lp, listener);
        }
    }
}