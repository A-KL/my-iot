using System.Net;
using System.Reflection;

using Microsoft.Practices.Unity;

using Microsoft.Iot.Web;
using Microsoft.Iot.Web.Api;
using Microsoft.Iot.Web.FileSystem;

using System.Linq;
using System.Web.Http;
using Windows.Networking.Connectivity;
using WebServerDemo.Model;

namespace WebServerDemo
{


    public sealed partial class MainPage
    {
        private const string DefaultPage = "index.html";

        public MainPage()
        {
            InitializeComponent();

            var container = new UnityContainer();

            container.RegisterType<IWeatherService, FakeWeatherService>(new HierarchicalLifetimeManager());


            var assembly = this.GetType().GetTypeInfo().Assembly;

            var settings = new HttpConfiguration
            {
                DefaultPath = DefaultPage,
                DependencyResolver = new UnityResolver(container)
            };


            //UseStaticFiles
            settings.Listeners.Add(new FileSystemListener("/", "wwwroot"));
            settings.Listeners.Add(new WebApiListener(assembly)); // use attribute routing            

            var server = new MessagingServer(
                new WebServiceFactory(settings),
                new MessagingServerConfiguration(new HttpMessageFactory()));

            server.Start(new IPEndPoint(IPAddress.Parse(this.GetLocalIp()), 8000));
        }

        private static string GetLocalIp()
        {
            var icp = NetworkInformation.GetInternetConnectionProfile();

            if (icp?.NetworkAdapter == null) return null;
            var hostname =
                NetworkInformation.GetHostNames()
                    .SingleOrDefault(
                        hn =>
                            hn.IPInformation?.NetworkAdapter != null && hn.IPInformation.NetworkAdapter.NetworkAdapterId
                            == icp.NetworkAdapter.NetworkAdapterId);

            // the ip address
            return hostname?.CanonicalName;
        }
    }
}
