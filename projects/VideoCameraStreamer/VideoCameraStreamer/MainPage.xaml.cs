using System.Linq;
using System.Net;
using System.Text;
using System.Web.Http;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Griffin.Core.Net.Protocols.Http.MJpeg;
using Griffin.Net;
using Griffin.Net.Channels;
using Griffin.Net.Protocols.Http;
using Griffin.Net.Protocols.Http.MJpeg;
using Griffin.Net.Protocols.Http.WebSocket;
using Griffin.Networking.Web;
using Microsoft.Iot.Web;
using Microsoft.Iot.Web.FileSystem;
using Microsoft.Iot.Web.Streaming;
using Microsoft.Practices.Unity;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using VideoCameraStreamer.Models;

namespace VideoCameraStreamer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string DefaultPage = "index.html";

        private FilesFrameSource source;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            //this.Init();
            this.InitNetwork();
        }

        private StreamSocketListener socket;

        /// <summary>
        /// The init.
        /// </summary>
        private async void Init()
        {
            var mediaCapture = new MediaCapture();
            await mediaCapture.InitializeAsync();

            this.VideoSource.Source = mediaCapture;

            await mediaCapture.StartPreviewAsync();

            await Task.Run(() => TakeFrame(mediaCapture));
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        private async void Initialize()
        {
            var camera = new CameraModule();

            await camera.InitializeAsync();


            this.VideoSource.Source = camera.Source;

            await camera.Source.StartPreviewAsync();
        }

        private static async Task TakeFrame(MediaCapture media)
        {
            var sw = new Stopwatch();

            var previewProperties = media.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;

            if (previewProperties == null)
            {
                return;
            }

            while (true)
            {
                sw.Restart();

                var videoFrame = new VideoFrame(BitmapPixelFormat.Bgra8, (int)previewProperties.Width, (int)previewProperties.Height);

                // Capture the preview frame
                using (var currentFrame = await media.GetPreviewFrameAsync(videoFrame))
                {
                    // Collect the resulting frame
                    var previewFrame = currentFrame.SoftwareBitmap;

                    using (var stream = new InMemoryRandomAccessStream())
                    {
                        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                        encoder.SetSoftwareBitmap(previewFrame);

                        await encoder.FlushAsync();

                        var readStrem = stream.AsStreamForRead();
                        var dataLean = readStrem.Length;
                        var data = new byte[dataLean];

                        await readStrem.ReadAsync(data, 0, data.Length);
                    }
                }

                sw.Stop();

                Debug.WriteLine("Single frame: {0}Ms {1}Fps", sw.ElapsedMilliseconds, 1000.0 / sw.ElapsedMilliseconds);
            }
        }

        //private unsafe void EditPixels(SoftwareBitmap bitmap)
        //{
        //    // Effect is hard-coded to operate on BGRA8 format only
        //    if (bitmap.BitmapPixelFormat == BitmapPixelFormat.Bgra8)
        //    {
        //        // In BGRA8 format, each pixel is defined by 4 bytes
        //        const int BYTES_PER_PIXEL = 4;

        //        using (var buffer = bitmap.LockBuffer(BitmapBufferAccessMode.ReadWrite))
        //        using (var reference = buffer.CreateReference())
        //        {
        //            // Get a pointer to the pixel buffer
        //            byte* data;
        //            uint capacity;
        //            ((IMemoryBufferByteAccess)reference).GetBuffer(out data, out capacity);

        //            // Get information about the BitmapBuffer
        //            var desc = buffer.GetPlaneDescription(0);

        //            // Iterate over all pixels
        //            for (uint row = 0; row < desc.Height; row++)
        //            {
        //                for (uint col = 0; col < desc.Width; col++)
        //                {
        //                    // Index of the current pixel in the buffer (defined by the next 4 bytes, BGRA8)
        //                    var currPixel = desc.StartIndex + desc.Stride * row + BYTES_PER_PIXEL * col;

        //                    // Read the current pixel information into b,g,r channels (leave out alpha channel)
        //                    var b = data[currPixel + 0]; // Blue
        //                    var g = data[currPixel + 1]; // Green
        //                    var r = data[currPixel + 2]; // Red

        //                    // Boost the green channel, leave the other two untouched
        //                    data[currPixel + 0] = b;
        //                    data[currPixel + 1] = (byte)Math.Min(g + 80, 255);
        //                    data[currPixel + 2] = r;
        //                }
        //            }
        //        }
        //    }
        //}

        private void InitNetwork()
        {

            var imagesFolder =
                  Windows.ApplicationModel.Package.Current.InstalledLocation
                  .GetFolderAsync("Images\\Countdown")
                  .GetAwaiter()
                  .GetResult();

            this.source = new FilesFrameSource(imagesFolder);

            var container = new UnityContainer();

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

            //response.StreamSource = this.source;

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