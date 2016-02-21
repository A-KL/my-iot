namespace Griffin.Networking.Web
{
    using System.Net;
    using Griffin.Networking.Servers;

    public class WebServiceFactory : IServiceFactory
    {
        private readonly WebServiceSettings settings;

        public WebServiceFactory(WebServiceSettings settings)
        {
            this.settings = settings;
        }

        public INetworkService CreateClient(EndPoint remoteEndPoint)
        {
            return new WebService(this.settings);
        }
    }
}
