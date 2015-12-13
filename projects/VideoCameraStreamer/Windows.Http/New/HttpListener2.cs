namespace Windows.Http.New
{
    using global::System;
    using global::System.Collections;
    using global::System.Collections.Concurrent;
    using global::System.Net;
    using global::System.Threading;
    using global::System.Threading.Tasks;
    using Networking;
    using Networking.Sockets;

    public sealed class HttpListener2 : IDisposable
    {
        private readonly ConcurrentQueue<HttpConnection2> queue;

        private readonly StreamSocketListener streamSocketListener;

        private readonly ManualResetEvent queueWaitHandle;

        private AuthenticationSchemes auth_schemes;

        private HttpListenerPrefixCollection prefixes;

        private TaskCompletionSource<HttpListenerContext> completionSource;

        private Hashtable connections;

        private string realm;
        private bool listening;
        private bool disposed;

        public HttpListener2()
        {
            this.connections = Hashtable.Synchronized(new Hashtable());

            this.queueWaitHandle = new ManualResetEvent(false);

            //this.prefixes = new HttpListenerPrefixCollection(this);

            this.queue = new ConcurrentQueue<HttpConnection2>();

            this.auth_schemes = AuthenticationSchemes.Anonymous;

            this.completionSource = new TaskCompletionSource<HttpListenerContext>();

            this.streamSocketListener = new StreamSocketListener();
            this.streamSocketListener.ConnectionReceived += this.StreamSocketListener_ConnectionReceived;
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
            get { return this.realm; }
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

            //var hostname = this.Prefixes.First().

            await this.streamSocketListener.BindEndpointAsync(new HostName(""), "123");

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

           // this.Close(true);
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

           // this.Close(true);
            this.disposed = true;
        }

        public void Stop()
        {
            this.CheckDisposed();
            this.listening = false;
           // this.Close(false);
        }

        void IDisposable.Dispose()
        {
            if (this.disposed)
            {
                return;
            }

           // this.Close(true); //TODO: Should we force here or not?
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
                this.queueWaitHandle.WaitOne();
            }

            if (this.queue.Count == 0)
            {
                this.queueWaitHandle.Reset();
            }

            return context.GetContext().Result;
        }

        private void StreamSocketListener_ConnectionReceived(StreamSocketListener sender,
            StreamSocketListenerConnectionReceivedEventArgs args)
        {
            var connection = new HttpConnection2(args.Socket);

            this.queue.Enqueue(connection);

            this.queueWaitHandle.Set();
        }

        internal void CheckDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(GetType().ToString());
            }
        }
    }
}