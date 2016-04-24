using System;
using Windows.Storage;
using Windows.Storage.Streams;
using Griffin.Core.Net.Protocols.Http.Multipart;

namespace Microsoft.Iot.Web.Streaming
{
    public class ImageFileFrame : IMultipartFrame
    {
        public ImageFileFrame(IStorageFile file)
        {
            this.Data = FileIO.ReadBufferAsync(file).GetAwaiter().GetResult();
        }

        public void Dispose()
        {

        }

        public int Height { get; }

        public int Width { get; }

        public int DataSize
        {
            get { return (int)this.Data.Length; }
        }

        public IBuffer Data { get; }
    }
}
