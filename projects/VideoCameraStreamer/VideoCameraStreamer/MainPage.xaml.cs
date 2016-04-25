using System.Linq;
using System.Net;
using System.Text;
using System.Web.Http;
using Windows.Networking.Connectivity;
using Griffin.Net;
using Griffin.Net.Channels;
using Griffin.Net.Protocols.Http;
using Griffin.Net.Protocols.Http.WebSocket;
using Griffin.Networking.Web;
using Microsoft.Iot.Web;
using Microsoft.Iot.Web.FileSystem;
using Microsoft.Practices.Unity;
using System;
using Windows.UI.Xaml.Controls;
using Griffin.Core.Net.Protocols.Http.Multipart;
using VideoCameraStreamer.Models;

namespace VideoCameraStreamer
{
    public sealed partial class MainPage : Page
    {
        private const string DefaultPage = "index.html";

        private IFramesSource source;

        public MainPage()
        {

            var imagesFolder =
                    Windows.ApplicationModel.Package.Current.InstalledLocation
                    .GetFolderAsync("Images\\Countdown")
                    .GetAwaiter()
                    .GetResult();

          // this.source = new FilesFrameSource(imagesFolder);

            this.InitializeComponent();

            this.InitializeCamera();
        }

        private async void InitializeCamera()
        {
            var cameras = await CameraModule.DiscoverAsync();

            var camera = cameras[0];

            await camera.InitializeAsync();

            this.VideoSource.Source = camera.Source;

            await camera.Start();

            this.source = new CameraFramesSource(camera);

            this.InitNetwork();
        }

        private void InitNetwork()
        {
            var container = new UnityContainer();

            var ip = IPAddress.Parse(NetworkHelper.GetLocalIp());

            // container.RegisterType<IWeatherService, FakeWeatherService>(new HierarchicalLifetimeManager());

            //var assembly = this.GetType().GetTypeInfo().Assembly;

            var settings = new HttpConfiguration
            {
                DefaultPath = DefaultPage,
                DependencyResolver = new UnityResolver(container)
            };
            
            //UseStaticFiles
            settings.Listeners.Add(new FileSystemListener("/", "wwwroot"));
            //settings.Listeners.Add(new WebApiListener(assembly)); // use attribute routing            

            // Http
            var server = new WebService(settings);
            server.Start(ip, 8000);

            //WebSockets
            var socket = new WebSocketListener();
            socket.Start(ip, 8001);
            socket.WebSocketMessageReceived = this.MessageReceived;

            //MJpeg
            var config = new ChannelTcpListenerConfiguration(
                    () => new HttpMessageDecoder(),
                    () => new MultipartEncoder());

            var liveVideoListeren = new HttpListener(config);
            liveVideoListeren.MessageReceived = this.LiveStreamMessageReceived;
            liveVideoListeren.Start(ip, 8002);
        }

        private void LiveStreamMessageReceived(ITcpChannel channel, object message)
        {
            var response = new HttpStreamResponse(HttpStatusCode.OK, "OK", "HTTP/1.1");

            response.StreamSource = this.source;

            channel.Send(response);
        }

        private void MessageReceived(ITcpChannel channel, object message)
        {
            var msg = message as IWebSocketMessage;

            var data = new byte[msg.Payload.Length];

            msg.Payload.Read(data, 0, (int)msg.Payload.Length);

            var text = Encoding.ASCII.GetString(data);

            //this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            //    this.Label.Text = text;
            //});
        }
    }
}