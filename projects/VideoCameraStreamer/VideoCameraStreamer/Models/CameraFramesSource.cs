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

        private readonly Stopwatch watch = new Stopwatch();

        public CameraFramesSource(CameraModule module)
        {
            this.module = module;
        }

        public async Task<bool> WriteNextFrame(MultipartStream stream)
        {
            watch.Restart();

            using (var frame = await this.module.ShootFrame())
            {
                if (frame == null)
                {
                    return false;
                }

                watch.Stop();

                Debug.WriteLine("Frame shooting: " + watch.ElapsedMilliseconds);
                
                //var frameSize = frame.SoftwareBitmap.PixelWidth * frame.SoftwareBitmap.PixelHeight * 4;
                //var temb = new Windows.Storage.Streams.Buffer((uint) frameSize);
                // var buffer = frame.SoftwareBitmap.LockBuffer(BitmapBufferAccessMode.Read);
                // var data = temb.ToArray();
                // stream.Write(data, 0, data.Length);
                
               // using (var tempStream = new InMemoryRandomAccessStream())
                {
                    //watch.Restart();

                    //var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, tempStream);
                    
                    //encoder.SetSoftwareBitmap(frame.SoftwareBitmap);

                    //await encoder.FlushAsync();

                    //watch.Stop();

                    //Debug.WriteLine("Frame Encoding: " + watch.ElapsedMilliseconds);

                    watch.Restart();

                    frame.AsStream().CopyTo(stream);

                    watch.Stop();

                    Debug.WriteLine("Frame copying: " + watch.ElapsedMilliseconds);

                }                               
            }

            return true;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
