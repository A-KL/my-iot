namespace System.Net
{
    using IO;
    using Sockets;
    using Text;
    using Threading;
    using global::Windows.Networking.Sockets;
    using global::Windows.Storage.Streams;
    using Threading.Tasks;

    enum InputState
    {
        RequestLine,
        Headers
    }

    enum LineState
    {
        None,
        CR,
        LF
    }

    public sealed class HttpConnection
    {
        private StreamSocket streamSocket;

        private MemoryStream ms;

        private EndPointListener epl;

        private HttpListenerContext context;
    
        private int s_timeout = 90000; // 90k ms for first request, 15k ms from then on
        private Timer timer;

        private IPEndPoint local_ep;
        private HttpListener last_listener;

        public HttpConnection(StreamSocket socket)
        {
            this.streamSocket = socket;

            this.timer = new Timer(this.OnTimeout, null, Timeout.Infinite, Timeout.Infinite);
            this.Init();
        }

        private void Init()
        {            
            this.Prefix = null;            
            this.ms = new MemoryStream();
            this.context = new HttpListenerContext(this);
        }

        public bool IsClosed
        {
            get { return (this.streamSocket == null); }
        }

        public int Reuses { get; private set; }

        public IPEndPoint LocalEndPoint
        {
            get
            {
                if (this.local_ep != null)
                {
                    return this.local_ep;
                }

                this.local_ep = new IPEndPoint(
                    IPAddress.Parse(this.streamSocket.Information.LocalAddress.RawName),
                    int.Parse(this.streamSocket.Information.LocalPort));

                return this.local_ep;
            }
        }

        public IPEndPoint RemoteEndPoint
        {
            get
            {
                return new IPEndPoint(
                       IPAddress.Parse(this.streamSocket.Information.RemoteAddress.RawName),
                       int.Parse(this.streamSocket.Information.RemotePort));
            }
        }

        public ListenerPrefix Prefix { get; set; }

        void OnTimeout(object unused)
        {
            CloseSocket();
            Unbind();
        }

        public void BeginReadRequest()
        {
            if (buffer == null)
            {
                buffer = new byte[BufferSize];
            }

            try
            {
                if (this.Reuses == 1)
                {
                    this.s_timeout = 15000;
                }
                this.timer.Change(s_timeout, Timeout.Infinite);
                this.stream.BeginRead(buffer, 0, BufferSize, onread_cb, this);
            }
            catch
            {
                this.timer.Change(Timeout.Infinite, Timeout.Infinite);
                this.CloseSocket();
                this.Unbind();
            }
        }

        public IOutputStream GetRequestStream(bool chunked, long contentlength)
        {
            //if (i_stream == null)
            //{
            //    byte[] buffer = ms.GetBuffer();
            //    int length = (int)ms.Length;
            //    ms = null;
            //    if (chunked)
            //    {
            //        this.chunked = true;
            //        context.Response.SendChunked = true;
            //        i_stream = new ChunkedInputStream(context, stream, buffer, position, length - position);
            //    }
            //    else
            //    {
            //        i_stream = new RequestStream(stream, buffer, position, length - position, contentlength);
            //    }
            //}
            //return i_stream;

            return this.streamSocket.OutputStream;
        }

        public IInputStream GetResponseStream()
        {
            // TODO: can we get this stream before reading the input?
            //if (o_stream == null)
            //{
            //    HttpListener listener = context.Listener;
            //    bool ign = (listener == null) ? true : listener.IgnoreWriteExceptions;
            //    o_stream = new ResponseStream(stream, context.Response, ign);
            //}
            //return o_stream;

            return this.streamSocket.InputStream;
        }

        private static void OnRead(IAsyncResult ares)
        {
            HttpConnection cnc = (HttpConnection)ares.AsyncState;
            cnc.OnReadInternal(ares);
        }

        private void OnReadInternal(IAsyncResult ares)
        {
            this.timer.Change(Timeout.Infinite, Timeout.Infinite);
            int nread = -1;
            try
            {
                nread = stream.EndRead(ares);
                ms.Write(buffer, 0, nread);
                if (ms.Length > 32768)
                {
                    SendError("Bad request", 400);
                    Close(true);
                    return;
                }
            }
            catch
            {
                if (ms != null && ms.Length > 0)
                    SendError();
                if (sock != null)
                {
                    CloseSocket();
                    Unbind();
                }
                return;
            }

            if (nread == 0)
            {
                //if (ms.Length > 0)
                //	SendError (); // Why bother?
                CloseSocket();
                Unbind();
                return;
            }

            if (ProcessInput(ms))
            {
                if (!context.HaveError)
                    context.Request.FinishInitialization();

                if (context.HaveError)
                {
                    SendError();
                    Close(true);
                    return;
                }

                if (!epl.BindContext(context))
                {
                    SendError("Invalid host", 400);
                    Close(true);
                    return;
                }
                HttpListener listener = context.Listener;
                if (last_listener != listener)
                {
                    RemoveConnection();
                    listener.AddConnection(this);
                    last_listener = listener;
                }

                context_bound = true;
                listener.RegisterContext(context);
                return;
            }
            stream.BeginRead(buffer, 0, BufferSize, onread_cb, this);
        }

        private void RemoveConnection()
        {
            if (last_listener == null)
                epl.RemoveConnection(this);
            else
                last_listener.RemoveConnection(this);
        }

        // true -> done processing
        // false -> need more input
        private bool ProcessInput(MemoryStream ms)
        {
            byte[] buffer = ms.GetBuffer();
            int len = (int)ms.Length;
            int used = 0;
            string line;

            try
            {
                line = ReadLine(buffer, position, len - position, ref used);
                position += used;
            }
            catch
            {
                context.ErrorMessage = "Bad request";
                context.ErrorStatus = 400;
                return true;
            }

            do
            {
                if (line == null)
                    break;
                if (line == "")
                {
                    if (input_state == InputState.RequestLine)
                        continue;
                    current_line = null;
                    ms = null;
                    return true;
                }

                if (input_state == InputState.RequestLine)
                {
                    context.Request.SetRequestLine(line);
                    input_state = InputState.Headers;
                }
                else
                {
                    try
                    {
                        context.Request.AddHeader(line);
                    }
                    catch (Exception e)
                    {
                        context.ErrorMessage = e.Message;
                        context.ErrorStatus = 400;
                        return true;
                    }
                }

                if (context.HaveError)
                    return true;

                if (position >= len)
                    break;
                try
                {
                    line = ReadLine(buffer, position, len - position, ref used);
                    position += used;
                }
                catch
                {
                    context.ErrorMessage = "Bad request";
                    context.ErrorStatus = 400;
                    return true;
                }
            } while (line != null);

            if (used == len)
            {
                ms.SetLength(0);
                position = 0;
            }
            return false;
        }

        string ReadLine(byte[] buffer, int offset, int len, ref int used)
        {
            if (current_line == null)
                current_line = new StringBuilder(128);
            int last = offset + len;
            used = 0;
            for (int i = offset; i < last && line_state != LineState.LF; i++)
            {
                used++;
                byte b = buffer[i];
                if (b == 13)
                {
                    line_state = LineState.CR;
                }
                else if (b == 10)
                {
                    line_state = LineState.LF;
                }
                else
                {
                    current_line.Append((char)b);
                }
            }

            string result = null;
            if (line_state == LineState.LF)
            {
                line_state = LineState.None;
                result = current_line.ToString();
                current_line.Length = 0;
            }

            return result;
        }

        public void SendError(string msg, int status)
        {
            try
            {
                HttpListenerResponse response = context.Response;
                response.StatusCode = status;
                response.ContentType = "text/html";
                string description = HttpListenerResponse.GetStatusDescription(status);
                string str;
                if (msg != null)
                    str = String.Format("<h1>{0} ({1})</h1>", description, msg);
                else
                    str = String.Format("<h1>{0}</h1>", description);

                byte[] error = context.Response.ContentEncoding.GetBytes(str);
                response.Close(error, false);
            }
            catch
            {
                // response was already closed
            }
        }

        public void SendError()
        {
            this.SendError(this.context.ErrorMessage, this.context.ErrorStatus);
        }

        private void Unbind()
        {
            if (context_bound)
            {
                epl.UnbindContext(context);
                context_bound = false;
            }
        }

        public void Close()
        {
            this.Close(false);
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
            { }
            finally
            {
                this.streamSocket = null;
            }
            this.RemoveConnection();
        }

        internal void Close(bool force_close)
        {
            if (this.streamSocket != null)
            {
                force_close |= !context.Request.KeepAlive;
                if (!force_close)
                {
                    force_close = (context.Response.Headers["connection"] == "close");
                }

                if (!force_close && context.Request.FlushInput())
                {
                    if (chunked && context.Response.ForceCloseChunked == false)
                    {
                        // Don't close. Keep working.
                        Reuses++;
                        Unbind();
                        Init();
                        BeginReadRequest();
                        return;
                    }

                    Reuses++;
                    Unbind();
                    Init();
                    BeginReadRequest();
                    return;
                }

                Socket s = sock;
                sock = null;
                try
                {
                    if (s != null)
                        s.Shutdown(SocketShutdown.Both);
                }
                catch
                {
                }
                finally
                {
                    if (s != null)
                        s.Close();
                }
                Unbind();
                RemoveConnection();
                return;
            }
        }
    }
}