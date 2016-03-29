using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

using VideoCameraStreamer.Streams;

namespace VideoCameraStreamer.UnitTests
{
    [TestClass]
    public class CustomStreamTest
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var filesDirectory = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\Countdown");

            using (var multiFilesStream = new DirectoryStream(filesDirectory))
            {
                
            }
        }
    }
}
