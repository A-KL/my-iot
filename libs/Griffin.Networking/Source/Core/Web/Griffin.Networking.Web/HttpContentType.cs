namespace Griffin.Networking.Web
{
    using System.Collections.Generic;

    public sealed class HttpContentType
    {
        private readonly static IDictionary<string, string> FileToContentMap = new Dictionary<string, string>
        {
            { ".html", Html }, { "jpeg", ImageJpeg }, { "js", JavaScript }, { "json", Json }
        };

        public const string Html = "text/html";
        public const string ImageJpeg = "image/jpeg";
        public const string ImagePng = "image/png";
        public const string ImageGif = "image/gif";
        public const string JavaScript = "application/javascript";
        public const string Json = "applicaton/json";

        public static string RolveFileExtension(string ext)
        {
            return FileToContentMap[ext];            
        }
    }
}
