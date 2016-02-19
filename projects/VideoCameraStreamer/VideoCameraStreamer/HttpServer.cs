namespace VideoCameraStreamer
{
    using System;
    using System.Threading.Tasks;
    using System.IO;
    using Windows.Networking.Sockets;
    using Windows.Storage.Streams;

    public class HttpServer
    {
        public async Task StartServer(int port)
        {
            var listener = new StreamSocketListener();

            await listener.BindServiceNameAsync(port.ToString());

            listener.ConnectionReceived += (s, e) =>
            {
                using (IOutputStream output = e.Socket.OutputStream)
                {
                    using (var writableStream = output.AsStreamForWrite())
                    {

                    }
                }
            };
        }

        //public event EventHandler<HttpRequestEventArgs> RequestReceived;

        //private void OnRequestReceived()
        //{
        //    if (this.RequestReceived != null)
        //    {
        //        this.RequestReceived(this, new HttpRequestEventArgs());
        //    }
        //}
    }
}
