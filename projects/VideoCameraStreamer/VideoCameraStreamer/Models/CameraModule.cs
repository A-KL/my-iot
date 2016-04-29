using System.Diagnostics;
using System.IO;
using System.Text;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace VideoCameraStreamer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.Devices.Enumeration;
    using System.Threading;
    using Windows.Graphics.Imaging;
    using Windows.Media;
    using Windows.Media.Capture;
    using Windows.Media.MediaProperties;

    public class CameraModule : IDisposable
    {
        private MediaCapture mediaCapture;

        private readonly DeviceInformation cameraDevice;

        private VideoEncodingProperties previewProperties;

        private CancellationTokenSource tokenSource;

        private VideoFrame lastVideoFrame;

        private CameraModule(DeviceInformation info)
        {
            this.cameraDevice = info;
        }


        public VideoFrame LastVideoFrame
        {
            get { return this.lastVideoFrame; }
            set { this.lastVideoFrame = value; }
        }

        public VideoEncodingProperties VideoProperties
        {
            get { return this.previewProperties; }
            set
            {
                if (null == value)
                {
                    return;
                }

                this.previewProperties = value;
            }
        }

        public MediaCapture Source => this.mediaCapture;


        public static async Task<IList<CameraModule>> DiscoverAsync()
        {
            var infos = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            var results = new List<CameraModule>(infos.Count);

            results.AddRange(infos.Select(information => new CameraModule(information)));

            return results;
        }

        public async Task<IRandomAccessStream> ShootFrame()
        {
           // var videoFrame = new VideoFrame(BitmapPixelFormat.Bgra8, (int)this.VideoProperties.Width, (int)this.VideoProperties.Height);

            var stream = new InMemoryRandomAccessStream();

            await this.mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);

            stream.Seek(0);

            return stream;
        }

        public async Task Start()
        {
            this.tokenSource = new CancellationTokenSource();

            //await this.capture.StartAsync();

            //this.mediaCapture.StartRecordToStreamAsync()
            // await this.mediaCapture.StartPreviewAsync();

            //Task.Run(async () =>
            //{
            //    while (!this.tokenSource.IsCancellationRequested)
            //    {
            //        var videoFrame = new VideoFrame(BitmapPixelFormat.Bgra8, (int)this.VideoProperties.Width, (int)this.VideoProperties.Height);

            //        await this.mediaCapture.GetPreviewFrameAsync(videoFrame);

            //        if (this.tokenSource.IsCancellationRequested)
            //        {
            //            break;
            //        }

            //        this.lastVideoFrame = videoFrame;
            //    }

            //    this.lastVideoFrame = null;

            //}, this.tokenSource.Token);

            // Add rotation metadata to the preview stream to make sure the aspect ratio / dimensions match when rendering and getting preview frames
            //var props = this.mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);

            

           // await this.mediaCapture.SetEncodingPropertiesAsync(MediaStreamType.VideoPreview, props, null);
        }

        public async Task InitializeAsync()
        {
            var settings = new MediaCaptureInitializationSettings
            {
                VideoDeviceId = this.cameraDevice.Id
            };

            this.mediaCapture = new MediaCapture();

            await this.mediaCapture.InitializeAsync(settings);

            //this.capture = await this.mediaCapture.PrepareLowLagPhotoCaptureAsync(ImageEncodingProperties.CreateJpeg());

           this.previewProperties = (VideoEncodingProperties)this.mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.Photo);
        }

        public IEnumerable<VideoEncodingProperties> GetAvailableResolutions()
        {
            return this.mediaCapture.VideoDeviceController
                .GetAvailableMediaStreamProperties(MediaStreamType.Photo)
                .Select(x => x as VideoEncodingProperties);
        }

        public void Dispose()
        {
            this.tokenSource?.Cancel();

            this.mediaCapture?.Dispose();
        }
    }
}
