using System.Threading.Tasks;

namespace Windows.Http
{
    using global::System;

    using Networking.Sockets;
    using Storage.Streams;

    public sealed class HttpConnection2 : IDisposable
    {
        private readonly StreamSocket streamSocket;

        public HttpConnection2(StreamSocket socket)
        {
            this.streamSocket = socket;
        }

        public ListenerPrefix Prefix { get; set; }

        internal Task<HttpListenerContext> GetContext()
        {
            return null;
        }


        public void Dispose()
        {
            this.streamSocket.Dispose();
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
    }
}
