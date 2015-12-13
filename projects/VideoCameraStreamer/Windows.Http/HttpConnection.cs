namespace Windows.Http
{
    using Extensions;
    using global::System;
    using global::System.Net;
    using global::System.Threading;

    using Networking.Sockets;
    using Storage.Streams;

    public sealed class HttpConnection
    {
        private StreamSocket streamSocket;
        private EndPointListener epl;
        private HttpListenerContext context;
        private Timer timer;

        private IPEndPoint localEndPoint;
        private HttpListener lastListener;

        public HttpConnection(StreamSocket sock, EndPointListener epl)
        {
            this.streamSocket = sock;
            this.epl = epl;

            this.timer = new Timer(this.OnTimeout, null, Timeout.Infinite, Timeout.Infinite);

            this.Prefix = null;
            this.context = new HttpListenerContext(this);
        }

        public bool IsClosed
        {
            get
            {
                return (this.streamSocket == null);
            }
        }

        public bool IsSecure
        {
            get { return false; }
        }

        public int Reuses
        {
            get; private set;
        }

        public IPEndPoint LocalEndPoint
        {
            get
            {
                if (this.localEndPoint == null)
                {
                    this.localEndPoint = this.streamSocket.Information.LocalEndPoint();
                }
                
                return this.localEndPoint;
            }
        }

        public IPEndPoint RemoteEndPoint
        {
            get
            {
                return this.streamSocket.Information.RemoteEndPoint();
            }
        }

        public ListenerPrefix Prefix
        {
            get; set;
        }

        public IOutputStream GetRequestStream()
        {
            return this.streamSocket.OutputStream;
        }

        public void Close()
        {
            this.Close(false);
        }

        public void SendError()
        {
            this.SendError(this.context.ErrorMessage, this.context.ErrorStatus);
        }

        public void SendError(string msg, int status)
        {
            //    try
            //    {
            //        HttpListenerResponse response = context.Response;
            //        response.StatusCode = status;
            //        response.ContentType = "text/html";
            //        string description = HttpListenerResponse.GetStatusDescription(status);
            //        string str;
            //        if (msg != null)
            //            str = String.Format("<h1>{0} ({1})</h1>", description, msg);
            //        else
            //            str = String.Format("<h1>{0}</h1>", description);

            //        byte[] error = context.Response.ContentEncoding.GetBytes(str);
            //        response.Close(error, false);
            //    }
            //    catch
            //    {
            //        // response was already closed
            //    }
        }

        public async void BeginReadRequest()
        {
            try
            {
               this.timer.Change(15000, Timeout.Infinite);

                var firstLine = await this.streamSocket.InputStream.ReadLine();

                //stream.BeginRead(buffer, 0, BufferSize, onread_cb, this);
            }
            catch
            {
                this.timer.Change(Timeout.Infinite, Timeout.Infinite);
                this.CloseSocket();
                this.Unbind();
            }
        }

        private void OnTimeout(object unused)
        {
            this.CloseSocket();
            this.Unbind();
        }

        private static void OnRead(IAsyncResult ares)
        {
            var cnc = (HttpConnection)ares.AsyncState;
            cnc.OnReadInternal(ares);
        }

        private void OnReadInternal(IAsyncResult ares)
        {
            this.timer.Change(Timeout.Infinite, Timeout.Infinite);
            int nread = -1;
            //try
            //{
            //    nread = stream.EndRead(ares);
            //    ms.Write(buffer, 0, nread);
            //    if (ms.Length > 32768)
            //    {
            //        SendError("Bad request", 400);
            //        Close(true);
            //        return;
            //    }
            //}
            //catch
            //{
            //    if (ms != null && ms.Length > 0)
            //    {
            //        this.SendError();
            //    }
            //    if (sock != null)
            //    {
            //        this.CloseSocket();
            //        this.Unbind();
            //    }
            //    return;
            //}

            if (nread == 0)
            {
                this.CloseSocket();
                this.Unbind();

                return;
            }

           // if (ProcessInput(ms))
            {
                if (!this.context.HaveError)
                {
                   // this.context.Request.FinishInitialization();
                }

                if (this.context.HaveError)
                {
                    this.SendError();
                    this.Close(true);
                    return;
                }

                if (!this.epl.BindContext(this.context))
                {
                    this.SendError("Invalid host", 400);
                    this.Close(true);
                    return;
                }

                var listener = this.context.Listener;

                if (this.lastListener != listener)
                {
                    this.RemoveConnection();
                    listener.AddConnection(this);
                    this.lastListener = listener;
                }

                listener.RegisterContext(this.context);
                return;
            }

           // stream.BeginRead(buffer, 0, BufferSize, onread_cb, this);
        }

        private void RemoveConnection()
        {
            if (this.lastListener == null)
            {
                this.epl.RemoveConnection(this);
            }
            else
            {
                this.lastListener.RemoveConnection(this);
            }
        }
        
        private void Unbind()
        {
            //if (context_bound)
            //{
            //    epl.UnbindContext(context);
            //    context_bound = false;
            //}
        }

        private void CloseSocket()
        {
            if (this.streamSocket == null)
            {
                return;
            }

            try
            {
                this.streamSocket.Dispose();
            }
            catch
            {
                // ignored
            }
            finally
            {
                this.streamSocket = null;
            }

            this.RemoveConnection();
        }

        internal void Close(bool force_close)
        {
            //if (sock != null)
            //{
            //    Stream st = GetResponseStream();
            //    if (st != null)
            //        st.Close();

            //    o_stream = null;
            //}

            //if (sock != null)
            //{
            //    force_close |= !context.Request.KeepAlive;
            //    if (!force_close)
            //        force_close = (context.Response.Headers["connection"] == "close");


            //    if (!force_close && context.Request.FlushInput())
            //    {
            //        if (chunked && context.Response.ForceCloseChunked == false)
            //        {
            //            // Don't close. Keep working.
            //            Reuses++;
            //            Unbind();
            //            Init();
            //            BeginReadRequest();
            //            return;
            //        }

            //        Reuses++;
            //        Unbind();
            //        Init();
            //        BeginReadRequest();
            //        return;
            //    }

            //    Socket s = sock;
            //    sock = null;
            //    try
            //    {
            //        if (s != null)
            //            s.Shutdown(SocketShutdown.Both);
            //    }
            //    catch
            //    {
            //    }
            //    finally
            //    {
            //        if (s != null)
            //            s.Close();
            //    }
            //    Unbind();
            //    RemoveConnection();
           // }
        }
    }
}