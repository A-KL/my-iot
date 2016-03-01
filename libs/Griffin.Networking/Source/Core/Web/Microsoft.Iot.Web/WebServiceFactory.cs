using System.Net;
using System.Web.Http;
using Griffin.Networking.Servers;
using Griffin.Networking.Web;

namespace Microsoft.Iot.Web
{
    public class WebServiceFactory : IServiceFactory
    {
        private readonly HttpConfiguration settings;

        public WebServiceFactory(HttpConfiguration settings)
        {
            this.settings = settings;
        }

        public INetworkService CreateClient(EndPoint remoteEndPoint)
        {
            return new WebService(this.settings);
        }
    }
}
