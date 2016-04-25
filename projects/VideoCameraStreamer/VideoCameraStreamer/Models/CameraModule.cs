﻿using System;
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

        private VideoFrame videoFrame;

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

            var previewProperties = (VideoEncodingProperties)this.mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);

            this.videoFrame = new VideoFrame(BitmapPixelFormat.Bgra8, (int)previewProperties.Width, (int)previewProperties.Height);
        }

        public IAsyncOperation<VideoFrame> ShootFrame()
        {
            if (!this.capturing)
            {
                this.mediaCapture.StartPreviewAsync().GetAwaiter().GetResult();
                this.capturing = true;
            }
            
            // Capture the preview frame
            return this.mediaCapture.GetPreviewFrameAsync(this.videoFrame);
        }

        //public IEnumerable<Task<VideoFrame>> Frames
        //{
        //    get
        //    {
        //        if (!this.capturing)
        //        {
        //            this.mediaCapture.StartPreviewAsync().GetAwaiter().GetResult();
        //            this.capturing = true;
        //        }

        //        var previewProperties =
        //            this.mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as
        //                VideoEncodingProperties;

        //        if (previewProperties == null)
        //        {
        //            yield return Task.FromResult<VideoFrame>(null);
        //        }

        //        var videoFrame = new VideoFrame(
        //            BitmapPixelFormat.Bgra8,
        //            (int) previewProperties.Width,
        //            (int) previewProperties.Height);

        //        // Capture the preview frame
        //        yield return this.mediaCapture.GetPreviewFrameAsync(videoFrame).AsTask();
        //    }
        //}

        public void Dispose()
        {            
            this.mediaCapture?.Dispose();
        }

        //public async Task TakeFrame()
        //{
        //    var previewProperties = this.mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;

        //    if (previewProperties == null)
        //    {
        //        return;
        //    }

        //    while (true)
        //    {
        //        var videoFrame = new VideoFrame(BitmapPixelFormat.Bgra8, (int) previewProperties.Width,
        //            (int) previewProperties.Height);

        //        // Capture the preview frame
        //        using (var currentFrame = await this.mediaCapture.GetPreviewFrameAsync(videoFrame))
        //        {
        //            // Collect the resulting frame
        //            var previewFrame = currentFrame.SoftwareBitmap;

        //            using (var stream = new InMemoryRandomAccessStream())
        //            {
        //                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
        //                encoder.SetSoftwareBitmap(previewFrame);

        //                await encoder.FlushAsync();

        //                var readStrem = stream.AsStreamForRead();
        //                var dataLean = readStrem.Length;
        //                var data = new byte[dataLean];

        //                await readStrem.ReadAsync(data, 0, data.Length)
        //                    .ConfigureAwait(false);
        //            }
        //        }

        //    }
        //}
    }
}
