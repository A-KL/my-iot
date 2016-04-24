namespace VideoCameraStreamer.Models
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Windows.Graphics.Imaging;
    using Windows.Media;
    using Windows.Storage.Streams;

    public static class VideoFrameExtensions
    {
        public static async Task<int> ConvertTo(this SoftwareBitmap bitmap, Guid encoderId, IRandomAccessStream destination)
        {
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, destination);

            encoder.SetSoftwareBitmap(bitmap);

            await encoder.FlushAsync();

            return (int)destination.Position;
        }

        public static async Task<int> ConvertTo(this VideoFrame frame, Guid encoderId, byte[] destination)
        {
            // Collect the resulting frame
            var previewFrame = frame.SoftwareBitmap;

            using (var stream = new InMemoryRandomAccessStream())
            {
                var dataSize = previewFrame.ConvertTo(encoderId, stream).Result;

                var readStrem = stream.AsStreamForRead();

                if (destination.Length < dataSize)
                {
                    throw new ArgumentOutOfRangeException();
                }

                await readStrem.ReadAsync(destination, 0, dataSize)
                    .ConfigureAwait(false);

                return dataSize;
            }
        }
    }
}