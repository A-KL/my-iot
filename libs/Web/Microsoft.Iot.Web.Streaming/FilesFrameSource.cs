﻿namespace Microsoft.Iot.Web.Streaming
{
    using System.IO;
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Griffin.Core.Net.Protocols.Http.Multipart;
    using Windows.Storage;

    public class FilesFrameSource : IFramesSource
    {
        private readonly IStorageFolder rootFolder;
        private IReadOnlyList<StorageFile> files;
        private int lastFileSentIndex;

        public FilesFrameSource(IStorageFolder folder)
        {
            this.rootFolder = folder;
        }

        public Task<bool> WriteNextFrame(Stream stream)
        {
            if (this.files == null)
            {
                this.files = this.rootFolder.GetFilesAsync().GetAwaiter().GetResult();
            }

            var file = this.files[this.lastFileSentIndex];

            //var buffer = FileIO. ReadBufferAsync(file).GetAwaiter().GetResult();

            // stream.Write(buffer.ToArray(), 0, (int)buffer.Length);

            using (var fileStream = file.OpenStreamForReadAsync().GetAwaiter().GetResult())
            {
                fileStream.CopyTo(stream);
            }

            Task.Delay(1000).Wait();

            this.lastFileSentIndex++;

            return Task.FromResult<bool>(this.files.Count > this.lastFileSentIndex);
        }

        public void Dispose()
        {   }
    }
}
