using System.Net;
using Griffin.Networking.Protocol.Http;
using Windows.Networking.Sockets;
using Griffin.Networking.Messaging;
using VideoCameraStreamer.Networking;

namespace VideoCameraStreamer
{
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

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
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

        private void InitOwinServer()
        {
            //const string baseUrl = "http://localhost:5000/";

            //using (WebApp.Start<Startup>(url: baseUrl))
            //{
            //    // Create HttpCient and make a request to api/values 

            //}
        }

        private async void InitNetwork()
        {
            var server = new MessagingServer(
                new MyHttpServiceFactory(),
                new MessagingServerConfiguration(new HttpMessageFactory()));

            server.Start(new IPEndPoint(new IPAddress( new byte[] { 192, 168, 1, 12}), 8000));

            //socket = new StreamSocketListener();

            //await socket.BindEndpointAsync(new HostName("192.168.1.117"), 8001.ToString());

            //var hosts = NetworkInformation.GetHostNames();
            //foreach (var hostName in hosts)
            //{
            //    Debug.WriteLine(hostName);
            //}

            //socket.ConnectionReceived += (e, d) =>
            //{
            //    var s = d.Socket;
            //};

            //var listener = new HttpListener();
            //listener.Prefixes.Add("http://127.0.0.1:8006/");


            //listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;

            //await listener.Start();

            //var context = await listener.GetContextAsync();

            //var newFile = await DownloadsFolder.CreateFileAsync("test.jpeg");

            //using (var fileStream = await newFile.OpenStreamForWriteAsync())
            //{
            //    await fileStream.WriteAsync(data, 0, data.Length);
            //}
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
    }
}