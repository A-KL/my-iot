namespace Griffin.Networking.Web
{
    using System.Collections.Generic;

    public sealed class HttpContentType
    {
        private readonly static IDictionary<string, string> FileToContentMap = new Dictionary<string, string>
        {
            { ".html", Html }, { ".jpeg", ImageJpeg }, { ".js", JavaScript }, { ".json", Json }, { ".css", Css }
        };

        public const string Html = "text/html";
        public const string ImageJpeg = "image/jpeg";
        public const string ImagePng = "image/png";
        public const string ImageGif = "image/gif";
        public const string JavaScript = "application/javascript";
        public const string Json = "applicaton/json";
        public const string Css = "text/css";

        public static string RolveFileExtension(string ext)
        {
            if (!FileToContentMap.ContainsKey(ext))
            {
                return null;
            }
            return FileToContentMap[ext];
        }
    }
}
