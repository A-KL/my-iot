using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace VideoCameraStreamer.Streams
{
    public class MultipartStream : Stream
    {
        public override bool CanRead { get; }

        public override bool CanSeek { get; }

        public override bool CanWrite { get; }

        public override long Length { get; }

        public override long Position { get; set; }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }

    public class MultipartStreamReader : IInputStream
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        IAsyncOperationWithProgress<IBuffer, uint> IInputStream.ReadAsync(IBuffer buffer, uint count, InputStreamOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
