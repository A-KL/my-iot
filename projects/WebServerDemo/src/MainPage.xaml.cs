using System;
using System.Net;
using System.Reflection;

using Microsoft.Practices.Unity;

using Microsoft.Iot.Web.Api;
using Microsoft.Iot.Web.FileSystem;

using System.Linq;
using System.Text;
using System.Web.Http;
using Windows.Networking.Connectivity;
using Windows.UI.Core;
using Griffin.Core.Net.Protocols.Http.MJpeg;
using Griffin.Net;
using Griffin.Net.Channels;
using Griffin.Net.Protocols.Http;
using Griffin.Net.Protocols.Http.MJpeg;
using Griffin.Net.Protocols.Http.WebSocket;
using Griffin.Networking.Web;
using WebServerDemo.Model;

namespace WebServerDemo
{
    public sealed partial class MainPage
    {
        private const string DefaultPage = "index.html";

        public IFramesSource source;

        public MainPage()
        {
            InitializeComponent();
            
            var imagesFolder =
                Windows.ApplicationModel.Package.Current.InstalledLocation
                .GetFolderAsync("Assets")
                .GetAwaiter()
                .GetResult();

            source = new FilesFrameSource(imagesFolder);

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

            // Http
            var server = new WebService(settings);
            server.Start(IPAddress.Parse(GetLocalIp()), 8000);

            //WebSockets
            var socket = new WebSocketListener();
            socket.Start(IPAddress.Parse(GetLocalIp()), 8001);
            socket.WebSocketMessageReceived = this.MessageReceived;

            //MJpeg
            var config = new ChannelTcpListenerConfiguration(
                    () => new HttpMessageDecoder(),
                    () => new MJpegEncoder());

            var liveVideoListeren = new HttpListener(config);
            liveVideoListeren.MessageReceived = this.LiveStreamMessageReceived;
            liveVideoListeren.Start(IPAddress.Parse(GetLocalIp()), 8002);           
        }

        private void LiveStreamMessageReceived(ITcpChannel channel, object message)
        {
            var response = new HttpStreamResponse(HttpStatusCode.OK, "ok", "HTTP/1.1");

            response.StreamSource = this.source;

            channel.Send(response);
        }

        private void MessageReceived(ITcpChannel channel, object message)
        {
            var msg = message as IWebSocketMessage;

            var data = new byte[msg.Payload.Length];

            msg.Payload.Read(data, 0, (int)msg.Payload.Length);

            var text = Encoding.ASCII.GetString(data);

            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.Label.Text = text;
            });
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
