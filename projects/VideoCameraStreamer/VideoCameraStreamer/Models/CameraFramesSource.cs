namespace VideoCameraStreamer.Models
{
    using System;
    using System.IO;
    using Griffin.Core.Net.Protocols.Http.Multipart;
    using Windows.Graphics.Imaging;
    using Windows.Storage.Streams;

    public class CameraFramesSource : IFramesSource
    {
        private readonly CameraModule module;

        public CameraFramesSource(CameraModule module)
        {
            this.module = module;
        }

        public bool WriteNextFrame(MultipartStream stream)
        {
            using (var frame = this.module.ShootFrame().GetAwaiter().GetResult())
            {
                if (frame == null)
                {
                    return false;
                }

                using (var tempStream = new InMemoryRandomAccessStream())
                {
                   var encoder =
                        BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, tempStream).GetAwaiter().GetResult();

                    encoder.SetSoftwareBitmap(frame.SoftwareBitmap);

                    encoder.FlushAsync().GetAwaiter().GetResult();

                    tempStream.AsStream().CopyTo(stream);
                }
            }
            return true;
        }
    }
}
