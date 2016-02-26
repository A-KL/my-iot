using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Windows.Storage;
using Griffin.Networking.Protocol.Http.Protocol;
using Griffin.Networking.Web.Listeners.WebApi;

namespace Griffin.Networking.Web.Listeners
{
    public class FileSystemListener : RouteListener
    {
        #region Private

        private readonly string filesRootDir;

        private const string DefaultPage = "index.html";

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
            //try
            //{
            //    var filePath = GetFilePath(uri, this.route) ?? DefaultPage;

            //    var rooFolder = await appInstalledFolder.GetFolderAsync(this.filesRootDir);

            //    await this.appInstalledFolder.GetFileAsync(fileName);
            //    return true;
            //}
            //catch
            //{
            //    return false;
            //}

            return false;
        }

        public override async Task<IResponse> ExecuteAsync(IRequest request)
        {
            try
            {
                var response = request.CreateResponse(HttpStatusCode.OK, "Welcome");

                var filePath = GetFilePath(request.Uri, route) ?? DefaultPage;
                
                var rooFolder = await appInstalledFolder.GetFolderAsync(this.filesRootDir);

                var fileStream = await rooFolder.OpenStreamForReadAsync(filePath);

                response.Body = fileStream;

                response.ContentType = HttpContentType.RolveFileExtension(Path.GetExtension(filePath)) ?? string.Empty;

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
