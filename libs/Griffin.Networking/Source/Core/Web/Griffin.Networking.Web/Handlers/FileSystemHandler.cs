namespace Griffin.Networking.Web.Handlers
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    using Protocol.Http.Protocol;

    public class FileSystemHandler : RouteHandler
    {
        private readonly string filesRootDir;

        public FileSystemHandler(string root)
        {
            this.filesRootDir = root;
        }

        public async override Task<IResponse> ExecuteAsync(string localPath, IRequest request)
        {
            var response = request.CreateResponse(HttpStatusCode.OK, "Welcome");

            var filePath = GetFilePath(request.Uri, localPath);

            var appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;

            var rooFolder = await appInstalledFolder.GetFolderAsync(this.filesRootDir);

            var fileStream = await rooFolder.OpenStreamForReadAsync("index.html");

            response.Body = fileStream;

            response.ContentType = HttpContentType.RolveFileExtension(Path.GetExtension(filePath) ?? "html");

            return response;
        }

        private static string GetFilePath(Uri uri, string localPath)
        {
            var host = new Uri(uri.Host);
            var masterUri = new Uri(host, localPath);

            return uri.MakeRelativeUri(masterUri).ToString();
        }
    }
}
