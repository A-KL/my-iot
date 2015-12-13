namespace Windows.Http
{
    using Foundation;
    using global::System;
    using global::System.Collections;
    using global::System.Collections.Generic;
    using global::System.Net;
    using global::System.Threading;

    using Networking;
    using Networking.Sockets;

    public sealed class EndPointListener
    {
        private readonly Dictionary<HttpConnection, HttpConnection> unregistered;

        private readonly IPEndPoint endpoint;
        private readonly StreamSocketListener streamSocketListener;

        private Dictionary<ListenerPrefix, HttpListener> prefixes;
        private List<ListenerPrefix> unhandled; // List<ListenerPrefix> unhandled; host = '*'
        private List<ListenerPrefix> all; // List<ListenerPrefix> all;  host = '+'

        public EndPointListener(IPAddress address, int port)
        {
            this.prefixes = new Dictionary<ListenerPrefix, HttpListener>();
            this.unregistered = new Dictionary<HttpConnection, HttpConnection>();
            this.endpoint = new IPEndPoint(address, port);

            this.streamSocketListener = new StreamSocketListener();
            this.streamSocketListener.ConnectionReceived += this.StreamSocketListener_ConnectionReceived;
        }

        public IAsyncAction Bind()
        {

            return this.streamSocketListener.BindServiceNameAsync(this.endpoint.Port.ToString());

            return this.streamSocketListener.BindEndpointAsync(
                new HostName(this.endpoint.Address.ToString()),
                this.endpoint.Port.ToString());
        }

        public bool BindContext(HttpListenerContext context)
        {
            var req = context.Request;

            ListenerPrefix prefix;
            var listener = this.SearchListener(req.Url, out prefix);
            if (listener == null)
            {
                return false;
            }

            context.Listener = listener;
            context.Connection.Prefix = prefix;

            return true;
        }

        public void UnbindContext(HttpListenerContext context)
        {
            if (context == null || context.Request == null)
            {
                return;
            }

            // context.Listener.UnregisterContext(context);
        }

        private void StreamSocketListener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            var conn = new HttpConnection(args.Socket, this);

            lock (this.unregistered)
            {
                this.unregistered[conn] = conn;
            }

            conn.BeginReadRequest();
        }

        internal void RemoveConnection(HttpConnection conn)
        {
            lock (this.unregistered)
            {
                this.unregistered.Remove(conn);
            }
        }

        private HttpListener SearchListener(Uri uri, out ListenerPrefix prefix)
        {
            prefix = null;

            if (uri == null)
            {
                return null;
            }

            var host = uri.Host;
            var port = uri.Port;
            var path = HttpUtility.UrlDecode(uri.AbsolutePath);
            var pathSlash = path[path.Length - 1] == '/' ? path : path + "/";

            HttpListener bestMatch = null;
            var bestLength = -1;

            if (!string.IsNullOrEmpty(host))
            {
                var p_ro = prefixes;

                foreach (var p in p_ro.Keys)
                {
                    var ppath = p.Path;
                    if (ppath.Length < bestLength)
                    {
                        continue;
                    }

                    if (p.Host != host || p.Port != port)
                    {
                        continue;
                    }

                    if (path.StartsWith(ppath) || pathSlash.StartsWith(ppath))
                    {
                        bestLength = ppath.Length;
                        bestMatch = p_ro[p];
                        prefix = p;
                    }
                }
                if (bestLength != -1)
                    return bestMatch;
            }

            var list = unhandled;
            bestMatch = MatchFromList(host, path, list, out prefix);

            if (path != pathSlash && bestMatch == null)
            {
                bestMatch = MatchFromList(host, pathSlash, list, out prefix);
            }

            if (bestMatch != null)
            {
                return bestMatch;
            }

            list = all;
            bestMatch = MatchFromList(host, path, list, out prefix);

            if (path != pathSlash && bestMatch == null)
            {
                bestMatch = MatchFromList(host, pathSlash, list, out prefix);
            }

            return bestMatch;
        }

        private HttpListener MatchFromList(string host, string path, IEnumerable<ListenerPrefix> list, out ListenerPrefix prefix)
        {
            prefix = null;
            if (list == null)
            {
                return null;
            }

            HttpListener best_match = null;
            var best_length = -1;

            foreach (var p in list)
            {
                var ppath = p.Path;
                if (ppath.Length < best_length)
                {
                    continue;
                }

                if (path.StartsWith(ppath))
                {
                    best_length = ppath.Length;
                    best_match = p.Listener;
                    prefix = p;
                }
            }

            return best_match;
        }

        private static void AddSpecial(ICollection<ListenerPrefix> coll, ListenerPrefix prefix)
        {
            if (coll == null)
            {
                return;
            }

            foreach (var p in coll)
            {
                if (p.Path == prefix.Path)
                {
                    throw new HttpListenerException(400, "Prefix already in use.");
                }
            }

            coll.Add(prefix);
        }

        private static bool RemoveSpecial(IList<ListenerPrefix> coll, ListenerPrefix prefix)
        {
            if (coll == null)
            {
                return false;
            }

            var c = coll.Count;

            for (var i = 0; i < c; i++)
            {
                var p = coll[i];
                if (p.Path == prefix.Path)
                {
                    coll.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        private void CheckIfRemove()
        {
            if (this.prefixes.Count > 0)
            {
                return;
            }

            var list = this.unhandled;
            if (list != null && list.Count > 0)
            {
                return;
            }

            list = this.all;
            if (list != null && list.Count > 0)
            {
                return;
            }

            EndPointManager.RemoveEndPoint(this, this.endpoint);
        }

        public void Close()
        {
            this.streamSocketListener.Dispose();

            lock (this.unregistered)
            {
                var connections = new List<HttpConnection>(this.unregistered.Keys);

                foreach (var connection in connections)
                {
                    connection.Close(true);
                }

                this.unregistered.Clear();
            }
        }

        public void AddPrefix(ListenerPrefix prefix, HttpListener listener)
        {
            List<ListenerPrefix> current;
            List<ListenerPrefix> future;

            if (prefix.Host == "*")
            {
                do
                {
                    current = unhandled;
                    future = current ?? new List<ListenerPrefix>();
                    prefix.Listener = listener;
                    AddSpecial(future, prefix);
                }
                while (Interlocked.CompareExchange(ref unhandled, future, current) != current);

                return;
            }

            if (prefix.Host == "+")
            {
                do
                {
                    current = all;
                    future = current ?? new List<ListenerPrefix>();
                    prefix.Listener = listener;
                    AddSpecial(future, prefix);
                }
                while (Interlocked.CompareExchange(ref all, future, current) != current);
                return;
            }

            Dictionary<ListenerPrefix, HttpListener> prefs, p2;
            do
            {
                prefs = prefixes;
                if (prefs.ContainsKey(prefix))
                {
                    var other = prefs[prefix];
                    if (other != listener) // TODO: code.
                        throw new HttpListenerException(400, "There's another listener for " + prefix);
                    return;
                }
                p2 = prefs;
                p2[prefix] = listener;
            } while (Interlocked.CompareExchange(ref prefixes, p2, prefs) != prefs);
        }

        public void RemovePrefix(ListenerPrefix prefix, HttpListener listener)
        {
            List<ListenerPrefix> current;
            List<ListenerPrefix> future;
            if (prefix.Host == "*")
            {
                do
                {
                    current = unhandled;
                    future = current ?? new List<ListenerPrefix>();//(current != null) ? (ArrayList)current.Clone() : new ArrayList();
                    if (!RemoveSpecial(future, prefix))
                        break; // Prefix not found
                } while (Interlocked.CompareExchange(ref unhandled, future, current) != current);
                CheckIfRemove();
                return;
            }

            if (prefix.Host == "+")
            {
                do
                {
                    current = all;
                    future = current ?? new List<ListenerPrefix>(); //(current != null) ? (ArrayList)current.Clone() : new ArrayList();
                    if (!RemoveSpecial(future, prefix))
                        break; // Prefix not found
                } while (Interlocked.CompareExchange(ref all, future, current) != current);
                CheckIfRemove();
                return;
            }

            Dictionary<ListenerPrefix, HttpListener> prefs, p2;
            do
            {
                prefs = prefixes;
                if (!prefs.ContainsKey(prefix))
                    break;

                p2 = prefs; //(Hashtable)prefs.Clone();
                p2.Remove(prefix);
            }
            while (Interlocked.CompareExchange(ref prefixes, p2, prefs) != prefs);
            CheckIfRemove();
        }
    }
}