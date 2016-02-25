namespace Griffin.Networking.Web.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Protocol.Http.Protocol;
    using WebApi;

    public class FileSystemHandler : RouteHandler
    {
        #region Private

        private readonly string filesRootDir;

        private const string DefaultPage = "index.html";

        private const string route = "/";

        #endregion

        #region Public

        public FileSystemHandler(string root)
        {
            this.filesRootDir = root;
        }

        public override IEnumerable<string> Routes
        {
            get { yield return route; }
        }

        public override async Task<IResponse> ExecuteAsync(IRequest request)
        {
            try
            {
                var response = request.CreateResponse(HttpStatusCode.OK, "Welcome");

                var filePath = GetFilePath(request.Uri, route) ?? DefaultPage;

                var appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;

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
