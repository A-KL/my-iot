using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Griffin.Core.Net.Protocols.Http.MJpeg;

namespace Microsoft.Iot.Web.Streaming
{
    public class FilesFrameSource //: IFramesSource
    {
        private readonly IStorageFolder rootFolder;

        public FilesFrameSource(IStorageFolder folder)
        {
            this.rootFolder = folder;
        }

        public void Start()
        {
             

            //Task.Run(() =>
            //{
                
            //})
        }
        
        public IEnumerable<IImageFrame> Frames
        {
            get
            {
                foreach (var storageFile in this.rootFolder.GetFilesAsync().GetAwaiter().GetResult())
                {
                    yield return new ImageFileFrame(storageFile);
                }              
            }
        }
    }
}
