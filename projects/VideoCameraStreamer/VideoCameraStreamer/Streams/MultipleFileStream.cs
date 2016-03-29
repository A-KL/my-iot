using System.IO;
using System.Threading;
using Windows.Storage;

namespace VideoCameraStreamer.Streams
{
    public class DirectoryStream : Stream
    {
        private readonly CancellationToken loopToken;

        private Stream currentFileStream;

        private readonly StorageFolder folder;

        private readonly string[] files;

        private int index;


        public DirectoryStream(StorageFolder folder)
            : this(folder, CancellationToken.None)
        { }

        public DirectoryStream(StorageFolder folder, CancellationToken loopCancellationToken)
        {
            this.index = 0;
            this.folder = folder;
            //this.files = filePaths;

            this.loopToken = loopCancellationToken;

            this.OpenNextFile();
        }

        public bool IsLooped => this.loopToken != CancellationToken.None && !this.loopToken.IsCancellationRequested;

        public override bool CanSeek => this.currentFileStream.CanSeek;

        public override bool CanRead 
            => this.IsLooped
            || ((this.index == this.files.Length) && this.currentFileStream.CanRead);
        
        public override bool CanWrite => false;

        public override long Length => this.currentFileStream?.Length ?? 0;

        public override long Position
        {
            get
            {
                return this.currentFileStream?.Position ?? -1;
            }
            set
            {
                this.currentFileStream.Position = value;
            }
        }


        public override void Flush()
        {
            this.currentFileStream.Flush();            
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = this.currentFileStream.Read(buffer, offset, count);

            if (read < count)
            {
                this.OpenNextFile();
            }

            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.currentFileStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this.currentFileStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
        }

        private void OpenNextFile()
        {
            this.currentFileStream?.Dispose();

            if (this.IsLooped)
            {
                this.index = 0;
            }
            else if (this.index == this.files.Length)
            {
                this.currentFileStream = null;
                return;
            }

            this.currentFileStream = new FileStream(this.files[this.index++], FileMode.Open, FileAccess.Read);
        }
    }
}
