using System.Diagnostics;

using Windows.Storage.Streams;

namespace VideoCameraStreamer.Models
{
    using System;
    using System.IO;
    using Griffin.Core.Net.Protocols.Http.Multipart;
    using Windows.Graphics.Imaging;
    using System.Threading.Tasks;

    public class CameraFramesSource : IFramesSource
    {
        private readonly CameraModule module;

        private readonly byte[] buffer = new byte[65535];

        private readonly MemoryStream frameBuffer;

       // private readonly Stopwatch watch = new Stopwatch();

        public CameraFramesSource(CameraModule module)
        {
            this.module = module;
            this.frameBuffer = new MemoryStream(buffer);
        }

        public async Task<bool> WriteNextFrame(Stream stream)
        {
         //   watch.Restart();

            using (var frame = await this.module.ShootFrame())
            {
                if (frame == null)
                {
                    return false;
                }

               // watch.Stop();

              //  Debug.WriteLine("Frame shooting: " + watch.ElapsedMilliseconds);

                //var frameSize = frame.SoftwareBitmap.PixelWidth * frame.SoftwareBitmap.PixelHeight * 4;
                //var temb = new Windows.Storage.Streams.Buffer((uint) frameSize);
                // var buffer = frame.SoftwareBitmap.LockBuffer(BitmapBufferAccessMode.Read);
                // var data = temb.ToArray();
                // stream.Write(data, 0, data.Length);

                this.frameBuffer.SetLength(0);
                this.frameBuffer.Position = 0;

              //  watch.Restart();

                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, this.frameBuffer.AsRandomAccessStream());

                encoder.SetSoftwareBitmap(frame.SoftwareBitmap);

                await encoder.FlushAsync();

                //watch.Stop();

                //Debug.WriteLine("Frame Encoding: " + watch.ElapsedMilliseconds);

                //watch.Restart();
               // this.frameBuffer.SetLength(0);
                this.frameBuffer.Position = 0;
                this.frameBuffer.CopyTo(stream);

                //watch.Stop();

                //Debug.WriteLine("Frame copying: " + watch.ElapsedMilliseconds);                                               
            }

            return true;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
