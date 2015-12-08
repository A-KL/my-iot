using System.IO;
using Windows.Storage;
using Windows.Storage.Streams;

namespace VideoCameraStreamer
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Windows.Graphics.Imaging;
    using Windows.Media;
    using Windows.Media.Capture;
    using Windows.Media.MediaProperties;
    using Windows.UI.Xaml.Controls;

    //[ComImport]
    //[Guid("5b0d3235-4dba-4d44-865e-8f1d0e4fd04d")]
    //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //unsafe interface IMemoryBufferByteAccess
    //{
    //    void GetBuffer(out byte* buffer, out uint capacity);
    //}

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
            this.Init();
        }

        /// <summary>
        /// The init.
        /// </summary>
        private async void Init()
        {
            var mediaCapture = new MediaCapture();
            await mediaCapture.InitializeAsync();

            this.VideoSource.Source = mediaCapture;

            // Get information about the preview
            var previewProperties = mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;

            // Create the video frame to request a SoftwareBitmap preview frame
            var videoFrame = new VideoFrame(BitmapPixelFormat.Bgra8, (int)previewProperties.Width, (int)previewProperties.Height);

            await mediaCapture.StartPreviewAsync();

            // Capture the preview frame
            using (var currentFrame = await mediaCapture.GetPreviewFrameAsync(videoFrame))
            {
                // Collect the resulting frame
                SoftwareBitmap previewFrame = currentFrame.SoftwareBitmap;

                using (var stream = new InMemoryRandomAccessStream())
                {
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                    encoder.SetSoftwareBitmap(previewFrame);

                    await encoder.FlushAsync();

                    var readStrem = stream.AsStreamForRead();
                    var dataLean = readStrem.Length;
                    var data = new byte[dataLean];
                    await readStrem.ReadAsync(data, 0, data.Length);

                    StorageFile newFile = await DownloadsFolder.CreateFileAsync("test.jpeg");
                    using (var fileStream = await newFile.OpenStreamForWriteAsync())
                    {
                        await fileStream.WriteAsync(data, 0, data.Length);
                    }
                }

                
                // Add a simple green filter effect to the SoftwareBitmap
                // EditPixels(previewFrame);
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
