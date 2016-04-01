using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Windows.Storage;
using Griffin.Net.Protocols.Http;

namespace Microsoft.Iot.Web.FileSystem
{
    public class FileSystemListener : RouteListener
    {
        #region Private

        private readonly string filesRootDir;

        private readonly string route;

        private readonly StorageFolder appInstalledFolder;

        #endregion

        #region Public

        public FileSystemListener(string uriRoot, string dirRoot)
        {
            this.route = uriRoot;
            this.filesRootDir = dirRoot;
            this.appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
        }

        public override bool IsListeningTo(Uri uri)
        {
            if (!string.IsNullOrEmpty(Path.GetExtension(uri.LocalPath)))
            {
                return true;
            }

            return false;
        }

        public override async Task<IHttpResponse> ExecuteAsync(IHttpRequest request, IDependencyResolver resolver)
        {
            try
            {
                var response = request.CreateResponse(HttpStatusCode.OK, "OK");

                var filePath = GetFilePath(request.Uri, this.route);

                var rooFolder = await this.appInstalledFolder.GetFolderAsync(this.filesRootDir);

                var fileStream = await rooFolder.OpenStreamForReadAsync(filePath);

                response.Body = fileStream;

                response.ContentType = System.Web.MimeMapping.GetMimeMapping(filePath);

                return response;
            }
            catch (Exception error)
            {
                return request.CreateResponse(HttpStatusCode.NotFound, error.Message);
            }
        }

        #endregion

        private static string GetFilePath(Uri uri, string localPath)
        {
            var localUri = uri.LocalPath;

            var index = localUri.IndexOf(localPath) + 1;
            var relUri = localUri.Substring(index, localUri.Length - index);

            var path = relUri.Replace("/", "\\");

            return string.IsNullOrEmpty(path) ? null : path;
        }
    }
}
