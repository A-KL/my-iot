using System.Collections.Generic;

namespace Windows.Http
{
    using global::System;
    using global::System.Collections;
    using global::System.Collections.Concurrent;
    using global::System.Net;
    using global::System.Threading.Tasks;

    public sealed class HttpListener : IDisposable
    {
        private readonly ConcurrentQueue<HttpConnection2> queue;

        private AuthenticationSchemes auth_schemes;

        private HttpListenerPrefixCollection prefixes;

        private TaskCompletionSource<HttpListenerContext> completionSource;

        private Dictionary<HttpListenerContext, HttpListenerContext> contextRegistry; 

        private Hashtable connections;

        private string realm;
        private bool listening;
        private bool disposed;

        public HttpListener()
        {
            this.connections = Hashtable.Synchronized(new Hashtable());

            this.prefixes = new HttpListenerPrefixCollection(this);

            this.queue = new ConcurrentQueue<HttpConnection2>();

            this.auth_schemes = AuthenticationSchemes.Anonymous;

            this.completionSource = new TaskCompletionSource<HttpListenerContext>();
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

        public async Task Start()
        {
            this.CheckDisposed();

            if (this.listening)
            {
                return;
            }

            await EndPointManager.AddListener(this);

            this.listening = true;
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

        public void Stop()
        {
            this.CheckDisposed();
            this.listening = false;
            this.Close(false);
        }

        void IDisposable.Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.Close(true); //TODO: Should we force here or not?
            this.disposed = true;
        }


        public Task<HttpListenerContext> GetContextAsync()
        {
            this.CheckDisposed();

            if (!this.listening)
            {
                throw new InvalidOperationException("Please, call Start before using this method.");
            }

            if (this.queue.Count == 0)
            {
                this.completionSource = new TaskCompletionSource<HttpListenerContext>();

                return this.completionSource.Task;
            }

            HttpConnection2 connection;
            if (this.queue.TryDequeue(out connection))
            {
                return connection.GetContext();
            }

            return null;
        }

        public HttpListenerContext GetContext()
        {
            this.CheckDisposed();

            if (!this.listening)
            {
                throw new InvalidOperationException("Please, call Start before using this method.");
            }

            if (this.queue.Count == 0)
            {
                return null;
            }

            HttpConnection2 context;
            while (!this.queue.TryDequeue(out context))
            {
                //this.queueWaitHandle.WaitOne();
            }

            if (this.queue.Count == 0)
            {
               // this.queueWaitHandle.Reset();
            }

            return context.GetContext().Result;
        }

        private void Close(bool force)
        {
            this.CheckDisposed();
            EndPointManager.RemoveListener(this).Wait();
            this.Cleanup(force);
        }

        private void Cleanup(bool closeExisting)
        {
            //lock (registry)
            //{
            //    if (closeExisting)
            //    {
            //        // Need to copy this since closing will call UnregisterContext
            //        ICollection keys = registry.Keys;
            //        var all = new HttpListenerContext[keys.Count];
            //        keys.CopyTo(all, 0);
            //        registry.Clear();
            //        for (int i = all.Length - 1; i >= 0; i--)
            //            all[i].Connection.Close(true);
            //    }

                lock (this.connections.SyncRoot)
                {
                    var keys = this.connections.Keys;
                    var conns = new HttpConnection[keys.Count];
                    keys.CopyTo(conns, 0);
                    this.connections.Clear();

                    for (var i = conns.Length - 1; i >= 0; i--)
                    {
                        conns[i].Close(true);
                    }
                }

                //lock (ctx_queue)
                //{
                //    var ctxs = (HttpListenerContext[])ctx_queue.ToArray(typeof(HttpListenerContext));
                //    ctx_queue.Clear();
                //    for (int i = ctxs.Length - 1; i >= 0; i--)
                //        ctxs[i].Connection.Close(true);
                //}

                //lock (wait_queue)
                //{
                //    Exception exc = new ObjectDisposedException("listener");
                //    foreach (ListenerAsyncResult ares in wait_queue)
                //    {
                //        ares.Complete(exc);
                //    }
                //    wait_queue.Clear();
                //}
           // }
        }

        internal void CheckDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(GetType().ToString());
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

        internal void RegisterContext(HttpListenerContext context)
        {
            lock (this.contextRegistry)
            {
                this.contextRegistry[context] = context;
            }

            //lock (wait_queue)
            //{
            //    if (wait_queue.Count == 0)
            //    {
            //        lock (ctx_queue)
            //        {
            //            ctx_queue.Add(context);
            //        }
            //    }
            //    else
            //    {
            //        ares = (ListenerAsyncResult)wait_queue[0];
            //        wait_queue.RemoveAt(0);
            //    }
            //}

            //if (ares != null)
            //{
            //    ares.Complete(context);
            //}
        }

        internal void UnregisterContext(HttpListenerContext context)
        {
            lock (this.contextRegistry)
            {
                this.contextRegistry.Remove(context);
            }

            //lock (ctx_queue)
            //{
            //    int idx = ctx_queue.IndexOf(context);
            //    if (idx >= 0)
            //    {
            //        ctx_queue.RemoveAt(idx);
            //    }
            //}
        }
    }
}