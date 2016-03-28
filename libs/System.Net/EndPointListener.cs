using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace System.Net
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;

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

        public Task Bind()
        {
            return this.streamSocketListener.BindEndpointAsync(,this.);
        }

        private void StreamSocketListener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            var conn = new HttpConnection(args.Socket, this);

            lock (this.unregistered)
            {
                this.unregistered[conn] = conn;
            }
        }

        internal void RemoveConnection(HttpConnection conn)
        {
            lock (unregistered)
            {
                unregistered.Remove(conn);
            }
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

            context.Listener.UnregisterContext(context);
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
            var path_slash = path[path.Length - 1] == '/' ? path : path + "/";

            HttpListener bestMatch = null;
            var bestLength = -1;

            if (!string.IsNullOrEmpty(host))
            {
                var p_ro = prefixes;

                foreach (ListenerPrefix p in p_ro.Keys)
                {
                    var ppath = p.Path;
                    if (ppath.Length < bestLength)
                        continue;

                    if (p.Host != host || p.Port != port)
                        continue;

                    if (path.StartsWith(ppath) || path_slash.StartsWith(ppath))
                    {
                        bestLength = ppath.Length;
                        bestMatch = (HttpListener)p_ro[p];
                        prefix = p;
                    }
                }
                if (bestLength != -1)
                    return bestMatch;
            }

            var list = unhandled;
            bestMatch = MatchFromList(host, path, list, out prefix);

            if (path != path_slash && bestMatch == null)
            {
                bestMatch = MatchFromList(host, path_slash, list, out prefix);
            }

            if (bestMatch != null)
            {
                return bestMatch;
            }

            list = all;
            bestMatch = MatchFromList(host, path, list, out prefix);

            if (path != path_slash && bestMatch == null)
            {
                bestMatch = MatchFromList(host, path_slash, list, out prefix);
            }

            return bestMatch;
        }

        private HttpListener MatchFromList(string host, string path, ArrayList list, out ListenerPrefix prefix)
        {
            prefix = null;
            if (list == null)
                return null;

            HttpListener best_match = null;
            int best_length = -1;

            foreach (ListenerPrefix p in list)
            {
                string ppath = p.Path;
                if (ppath.Length < best_length)
                    continue;

                if (path.StartsWith(ppath))
                {
                    best_length = ppath.Length;
                    best_match = p.Listener;
                    prefix = p;
                }
            }

            return best_match;
        }

        private static void AddSpecial(ArrayList coll, ListenerPrefix prefix)
        {
            if (coll == null)
                return;

            foreach (ListenerPrefix p in coll)
            {
                if (p.Path == prefix.Path) //TODO: code
                    throw new HttpListenerException(400, "Prefix already in use.");
            }
            coll.Add(prefix);
        }

        private static bool RemoveSpecial(ArrayList coll, ListenerPrefix prefix)
        {
            if (coll == null)
                return false;

            int c = coll.Count;
            for (int i = 0; i < c; i++)
            {
                ListenerPrefix p = (ListenerPrefix)coll[i];
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
            if (prefixes.Count > 0)
                return;

            ArrayList list = unhandled;
            if (list != null && list.Count > 0)
                return;

            list = all;
            if (list != null && list.Count > 0)
                return;

            EndPointManager.RemoveEndPoint(this, endpoint);
        }

        public void Close()
        {
            this.streamSocketListener.Dispose();
            lock (unregistered)
            {
                //
                // Clone the list because RemoveConnection can be called from Close
                //
                var connections = new List<HttpConnection>(unregistered.Keys);

                foreach (var c in connections)
                {
                    c.Close(true);
                }
                unregistered.Clear();
            }
        }

        public void AddPrefix(ListenerPrefix prefix, HttpListener listener)
        {
            ArrayList current;
            ArrayList future;

            if (prefix.Host == "*")
            {
                do
                {
                    current = unhandled;
                    future = (current != null) ? (ArrayList)current.Clone() : new ArrayList();
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
                    future = (current != null) ? (ArrayList)current.Clone() : new ArrayList();
                    prefix.Listener = listener;
                    AddSpecial(future, prefix);
                } while (Interlocked.CompareExchange(ref all, future, current) != current);
                return;
            }

            Hashtable prefs, p2;
            do
            {
                prefs = prefixes;
                if (prefs.ContainsKey(prefix))
                {
                    HttpListener other = (HttpListener)prefs[prefix];
                    if (other != listener) // TODO: code.
                        throw new HttpListenerException(400, "There's another listener for " + prefix);
                    return;
                }
                p2 = (Hashtable)prefs.Clone();
                p2[prefix] = listener;
            } while (Interlocked.CompareExchange(ref prefixes, p2, prefs) != prefs);
        }

        public void RemovePrefix(ListenerPrefix prefix, HttpListener listener)
        {
            ArrayList current;
            ArrayList future;
            if (prefix.Host == "*")
            {
                do
                {
                    current = unhandled;
                    future = (current != null) ? (ArrayList)current.Clone() : new ArrayList();
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
                    future = (current != null) ? (ArrayList)current.Clone() : new ArrayList();
                    if (!RemoveSpecial(future, prefix))
                        break; // Prefix not found
                } while (Interlocked.CompareExchange(ref all, future, current) != current);
                CheckIfRemove();
                return;
            }

            Hashtable prefs, p2;
            do
            {
                prefs = prefixes;
                if (!prefs.ContainsKey(prefix))
                    break;

                p2 = (Hashtable)prefs.Clone();
                p2.Remove(prefix);
            } while (Interlocked.CompareExchange(ref prefixes, p2, prefs) != prefs);
            CheckIfRemove();
        }
    }
}