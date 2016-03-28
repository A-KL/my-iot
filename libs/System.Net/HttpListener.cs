namespace System.Net
{
    using System.Collections;
    using System.Threading.Tasks;

    public sealed class HttpListener : IDisposable
    {
        private AuthenticationSchemes auth_schemes;
        private HttpListenerPrefixCollection prefixes;

        private string realm;
        private bool listening;
        private bool disposed;

        Hashtable registry;   // Dictionary<HttpListenerContext,HttpListenerContext> 
        ArrayList ctx_queue;  // List<HttpListenerContext> ctx_queue;
        ArrayList wait_queue; // List<ListenerAsyncResult> wait_queue;
        Hashtable connections;

        public HttpListener()
        {
            this.prefixes = new HttpListenerPrefixCollection(this);

            this.registry = new Hashtable();
            this.connections = Hashtable.Synchronized(new Hashtable());
            this.ctx_queue = new ArrayList();
            this.wait_queue = new ArrayList();
            this.auth_schemes = AuthenticationSchemes.Anonymous;
        }

        public bool IsListening
        {
            get { return this.listening; }
        }

        public HttpListenerPrefixCollection Prefixes
        {
            get
            {
                this.CheckDisposed();
                return this.prefixes;
            }
        }

        public string Realm
        {
            get
            {
                return this.realm;
            }
            set
            {
                this.CheckDisposed();
                this.realm = value;
            }
        }

        public void Abort()
        {
            if (this.disposed)
            {
                return;
            }

            if (!this.listening)
            {
                return;
            }

            this.Close(true);
        }

        public void Close()
        {
            if (this.disposed)
            {
                return;
            }

            if (!this.listening)
            {
                this.disposed = true;
                return;
            }

            this.Close(true);
            this.disposed = true;
        }

        private void Close(bool force)
        {
            this.CheckDisposed();
            EndPointManager.RemoveListener(this);
            this.Cleanup(force);
        }

        private void Cleanup(bool closeExisting)
        {
            lock (this.registry)
            {
                if (closeExisting)
                {
                    // Need to copy this since closing will call UnregisterContext
                    var keys = this.registry.Keys;

                    var all = new HttpListenerContext[keys.Count];

                    keys.CopyTo(all, 0);

                    registry.Clear();

                    for (int i = all.Length - 1; i >= 0; i--)
                    {
                        all[i].Connection.Close(true);
                    }
                }

                lock (this.connections.SyncRoot)
                {
                    ICollection keys = this.connections.Keys;
                    var conns = new HttpConnection[keys.Count];
                    keys.CopyTo(conns, 0);
                    this.connections.Clear();
                    for (int i = conns.Length - 1; i >= 0; i--)
                    {
                        conns[i].Close(true);
                    }
                }

                lock (this.ctx_queue)
                {
                    var ctxs = (HttpListenerContext[])this.ctx_queue.ToArray(typeof(HttpListenerContext));
                    this.ctx_queue.Clear();
                    for (int i = ctxs.Length - 1; i >= 0; i--)
                    {
                        ctxs[i].Connection.Close(true);
                    }
                }

                lock (wait_queue)
                {
                    Exception exc = new ObjectDisposedException("listener");
                    foreach (ListenerAsyncResult ares in wait_queue)
                    {
                        ares.Complete(exc);
                    }
                    wait_queue.Clear();
                }
            }
        }

        public IAsyncResult BeginGetContext(AsyncCallback callback, Object state)
        {
            this.CheckDisposed();

            if (!this.listening)
            {
                throw new InvalidOperationException("Please, call Start before using this method.");
            }

            var ares = new ListenerAsyncResult(callback, state);

            // lock wait_queue early to avoid race conditions
            lock (this.wait_queue)
            {
                lock (ctx_queue)
                {
                    HttpListenerContext ctx = GetContextFromQueue();
                    if (ctx != null)
                    {
                        ares.Complete(ctx, true);
                        return ares;
                    }
                }

                wait_queue.Add(ares);
            }

            return ares;
        }

        public HttpListenerContext EndGetContext(IAsyncResult asyncResult)
        {
            CheckDisposed();
            if (asyncResult == null)
                throw new ArgumentNullException("asyncResult");

            ListenerAsyncResult ares = asyncResult as ListenerAsyncResult;
            if (ares == null)
                throw new ArgumentException("Wrong IAsyncResult.", "asyncResult");
            if (ares.EndCalled)
                throw new ArgumentException("Cannot reuse this IAsyncResult");
            ares.EndCalled = true;

            if (!ares.IsCompleted)
                ares.AsyncWaitHandle.WaitOne();

            lock (wait_queue)
            {
                int idx = wait_queue.IndexOf(ares);
                if (idx >= 0)
                    wait_queue.RemoveAt(idx);
            }

            HttpListenerContext context = ares.GetContext();
            context.ParseAuthentication(SelectAuthenticationScheme(context));
            return context; // This will throw on error.
        }

        public HttpListenerContext GetContext()
        {
            // The prefixes are not checked when using the async interface!?
            if (prefixes.Count == 0)
                throw new InvalidOperationException("Please, call AddPrefix before using this method.");

            ListenerAsyncResult ares = (ListenerAsyncResult)BeginGetContext(null, null);
            ares.InGet = true;
            return EndGetContext(ares);
        }

        public Task<HttpListenerContext> GetContextAsync()
        {
            return Task<HttpListenerContext>.Factory.FromAsync(BeginGetContext, EndGetContext, null);
        }

        public void Start()
        {
            CheckDisposed();
            if (listening)
                return;

            EndPointManager.AddListener(this);
            listening = true;
        }

        public void Stop()
        {
            CheckDisposed();
            listening = false;
            Close(false);
        }

        void IDisposable.Dispose()
        {
            if (disposed)
                return;

            Close(true); //TODO: Should we force here or not?
            disposed = true;
        }



        internal void CheckDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().ToString());
        }

        // Must be called with a lock on ctx_queue
        private HttpListenerContext GetContextFromQueue()
        {
            if (ctx_queue.Count == 0)
                return null;

            HttpListenerContext context = (HttpListenerContext)ctx_queue[0];
            ctx_queue.RemoveAt(0);
            return context;
        }

        internal void RegisterContext(HttpListenerContext context)
        {
            lock (registry)
                registry[context] = context;

            ListenerAsyncResult ares = null;
            lock (wait_queue)
            {
                if (wait_queue.Count == 0)
                {
                    lock (ctx_queue)
                        ctx_queue.Add(context);
                }
                else
                {
                    ares = (ListenerAsyncResult)wait_queue[0];
                    wait_queue.RemoveAt(0);
                }
            }
            if (ares != null)
                ares.Complete(context);
        }

        internal void UnregisterContext(HttpListenerContext context)
        {
            lock (registry)
                registry.Remove(context);
            lock (ctx_queue)
            {
                int idx = ctx_queue.IndexOf(context);
                if (idx >= 0)
                    ctx_queue.RemoveAt(idx);
            }
        }

        internal void AddConnection(HttpConnection cnc)
        {
            connections[cnc] = cnc;
        }

        internal void RemoveConnection(HttpConnection cnc)
        {
            connections.Remove(cnc);
        }
    }
}