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

        // private readonly IRandomAccessStream frameStream;

        //   private readonly BitmapEncoder encoder;

        public CameraFramesSource(CameraModule module)
        {
            this.module = module;
            //this.frameStream = new InMemoryRandomAccessStream();

        }

        public bool WriteNextFrame(MultipartStream stream)
        {
            var frame = this.module.ShootFrame().GetAwaiter().GetResult();
            
                if (frame == null)
            {
                return false;
            }

            using (var tempStream = new InMemoryRandomAccessStream())
            {
                // this.frameStream.Seek(0);

                var encoder = BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, tempStream).GetAwaiter().GetResult();

                encoder.SetSoftwareBitmap(frame.SoftwareBitmap);

                encoder.FlushAsync().GetAwaiter().GetResult();

                //var dataSize = frame.SoftwareBitmap.ConvertTo(BitmapEncoder.JpegEncoderId, this.frameStream).Result;

                tempStream.AsStream().CopyTo(stream);
            }

            return true;
        }
    }
}
