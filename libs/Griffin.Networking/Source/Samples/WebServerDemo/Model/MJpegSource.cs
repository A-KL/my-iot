using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace WebServerDemo.Model
{
    internal class MJpegSource
    {
        public object Boundary { get; } = "HintDesk";

        public async Task WriteToStream(Stream outputStream, HttpContent content, TransportContext context)
        {
            byte[] newLine = Encoding.UTF8.GetBytes("\r\n");

            var filesDirectory = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\Countdown");
            
            foreach (var file in await filesDirectory.GetFilesAsync())
            {
                var properties = await file.GetBasicPropertiesAsync();

                var header = $"--{Boundary}\r\nContent-Type: image/jpeg\r\nContent-Length: {properties.Size}\r\n\r\n";
                var headerData = Encoding.UTF8.GetBytes(header);

                await outputStream.WriteAsync(headerData, 0, headerData.Length);

                using (var fileStream = await file.OpenStreamForWriteAsync())
                {
                    await fileStream.CopyToAsync(outputStream);
                }

                await outputStream.WriteAsync(newLine, 0, newLine.Length);

                await Task.Delay(1000);
            }
        }
    }
}
