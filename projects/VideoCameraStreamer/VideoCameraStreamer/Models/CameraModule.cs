using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;

namespace VideoCameraStreamer.Models
{
    /// <summary>
    /// The camera module.
    /// </summary>
    public class CameraModule : IDisposable
    {
        private MediaCapture mediaCapture;

        private readonly DeviceInformation cameraDevice;

        private VideoEncodingProperties previewProperties;

        private bool capturing;

        private CameraModule(DeviceInformation info)
        {
            this.cameraDevice = info;
            this.capturing = false;
        }

        public static async Task<IList<CameraModule>> DiscoverAsync()
        {
            var infos = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            var results = new List<CameraModule>(infos.Count);

            results.AddRange(infos.Select(information => new CameraModule(information)));

            return results;
        }

        public MediaCapture Source => this.mediaCapture;

        public IAsyncAction Start()
        {
            this.capturing = true;
            return this.mediaCapture.StartPreviewAsync();
        }

        public async Task InitializeAsync()
        {
            var settings = new MediaCaptureInitializationSettings
            {
                VideoDeviceId = this.cameraDevice.Id
            };

            this.mediaCapture = new MediaCapture();

            await this.mediaCapture.InitializeAsync(settings);

            previewProperties = (VideoEncodingProperties)this.mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);            
        }

        public IAsyncOperation<VideoFrame> ShootFrame()
        {
            if (!this.capturing)
            {
                this.mediaCapture.StartPreviewAsync().GetAwaiter().GetResult();
                this.capturing = true;
            }

            var videoFrame = new VideoFrame(BitmapPixelFormat.Bgra8, (int)previewProperties.Width, (int)previewProperties.Height);

            return this.mediaCapture.GetPreviewFrameAsync(videoFrame);
        }

        public void Dispose()
        {            
            this.mediaCapture?.Dispose();
        }
    }
}
