namespace VideoCameraStreamer.Networking
{
    using System.Net;
    using Griffin.Networking.Servers;

    public class MyHttpServiceFactory : IServiceFactory
    {
        public INetworkService CreateClient(EndPoint remoteEndPoint)
        {
            return new MyHttpService();
        }
    }
}