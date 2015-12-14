namespace Windows.Http
{
    using Windows.Http.Extensions;

    using global::System;
    using global::System.Collections.Specialized;
    using global::System.Net;
    using global::System.Text;

    using Storage.Streams;

    using Web.Http;

    public sealed class HttpListenerRequest
    {
        #region Members

        private static char[] separators = { ' ' };

        private WebHeaderCollection headers;

        private string path;

        private Uri url;

        private IOutputStream outputStream;

        private readonly HttpListenerContext context;

        private Encoding encoding;

        private long contentLength;

        #endregion        

        public HttpListenerRequest(HttpListenerContext context)
        {
            this.context = context;
            this.headers = new WebHeaderCollection();
            this.Cookies = new CookieCollection();
            this.Version = HttpVersion.Http10;
        }

        #region Properties

        public WebHeaderCollection Headers
        {
            get
            {
                return this.headers;
            }
        }

        public Uri Url
        {
            get
            {
                return this.url;
            }
        }

        public string HttpMethod { get; private set; }

        public HttpVersion Version { get; private set; }

        public string[] AcceptTypes { get; private set; }

        public Encoding ContentEncoding
        {
            get
            {
                if (this.encoding == null)
                {
                    this.encoding = Encoding.ASCII;
                }
                return this.encoding;
            }
        }

        public IOutputStream Content
        {
            get
            {
                if (this.outputStream == null)
                {
                    this.outputStream = this.context.Connection.GetRequestStream();
                }

                return this.outputStream;
            }
        }

        public bool IsSecureConnection
        {
            get { return this.context.Connection.IsSecure; }
        }

        public string RawUrl { get; set; }

        public long ContentLength64
        {
            get
            {
                return this.contentLength;
            }
        }

        public string UserAgent
        {
            get
            {
                return this.headers["user-agent"];
            }
        }

        public string UserHostAddress
        {
            get
            {
                return this.LocalEndPoint.ToString();
            }
        }

        public string UserHostName
        {
            get
            {
                return this.headers["host"];
            }
        }

        public string[] UserLanguages { get; private set; }

        public string ContentType
        {
            get
            {
                return this.headers["content-type"];
            }
        }

        public CookieCollection Cookies { get; }

        public bool IsAuthenticated
        {
            get
            {
                return false;
            }
        }

        public IPEndPoint RemoteEndPoint
        {
            get
            {
                return this.context.Connection.RemoteEndPoint;
            }
        }

        public IPEndPoint LocalEndPoint
        {
            get
            {
                return this.context.Connection.LocalEndPoint;
            }
        }

        public NameValueCollection QueryString
        {
            get; private set;
        }

        #endregion


        internal void SetRequestLine(string req)
        {
            var parts = req.Split(separators, 3);

            if (parts.Length != 3)
            {
                this.context.ErrorMessage = "Invalid request line (parts).";
                return;
            }

            this.HttpMethod = parts[0];

            foreach (var c in this.HttpMethod)
            {
                var ic = (int)c;

                if ((ic >= 'A' && ic <= 'Z')
                    || (ic > 32 && c < 127 && c != '(' && c != ')' && c != '<' && c != '<' && c != '>' && c != '@' && c != ',' && c != ';' && c != ':' && c != '\\'
                        && c != '"' && c != '/' && c != '[' && c != ']' && c != '?' && c != '=' && c != '{' && c != '}'))
                {
                    continue;
                }

                this.context.ErrorMessage = "(Invalid verb)";

                return;
            }

            this.RawUrl = parts[1];

            if (parts[2].Length != 8 || !parts[2].StartsWith("HTTP/"))
            {
                this.context.ErrorMessage = "Invalid request line (version).";
                return;
            }

            try
            {
                this.Version = HttpVersion.None;
                HttpVersion version;

                if (Enum.TryParse(parts[2].Substring(5), true, out version))
                {
                    this.Version = version;
                }
            }
            catch
            {
                this.context.ErrorMessage = "Invalid request line (version).";
            }
        }

        internal void AddHeader(string header)
        {
            var colon = header.IndexOf(':');

            if (colon == -1 || colon == 0)
            {
                this.context.ErrorMessage = "Bad Request";
                this.context.ErrorStatus = 400;

                return;
            }

            var name = header.Substring(0, colon).Trim();
            var val = header.Substring(colon + 1).Trim();
            var lower = name.ToLower();

            this.headers[name] = val;

            switch (lower)
            {
                case "accept-language":
                    this.UserLanguages = val.Split(','); // yes, only split with a ','
                    break;

                case "accept":
                    this.AcceptTypes = val.Split(','); // yes, only split with a ','
                    break;

                case "content-length":
                    try
                    {
                        this.contentLength = int.Parse(val.Trim());
                        if (this.contentLength < 0)
                        {
                            this.context.ErrorMessage = "Invalid Content-Length.";
                        }
                        //cl_set = true;
                    }
                    catch
                    {
                        this.context.ErrorMessage = "Invalid Content-Length.";
                    }
                    break;

                //case "referer":
                //    try
                //    {
                //        referrer = new Uri(val);
                //    }
                //    catch
                //    {
                //        referrer = new Uri("http://someone.is.screwing.with.the.headers.com/");
                //    }
                //    break;

                case "cookie":
                    var cookieStrings = val.Split(new[] { ',', ';' });

                    Cookie current = null;
                    var version = 0;

                    foreach (var cookieString in cookieStrings)
                    {
                        var str = cookieString.Trim();

                        if (str.Length == 0)
                        {
                            continue;
                        }

                        if (str.StartsWith("$Version"))
                        {
                            version = int.Parse(Unquote(str.Substring(str.IndexOf('=') + 1)));
                        }
                        else if (str.StartsWith("$Path"))
                        {
                            if (current != null)
                            {
                                current.Path = str.Substring(str.IndexOf('=') + 1).Trim();
                            }
                        }
                        else if (str.StartsWith("$Domain"))
                        {
                            if (current != null)
                            {
                                current.Domain = str.Substring(str.IndexOf('=') + 1).Trim();
                            }
                        }
                        else if (str.StartsWith("$Port"))
                        {
                            if (current != null)
                            {
                                current.Port = str.Substring(str.IndexOf('=') + 1).Trim();
                            }
                        }
                        else
                        {
                            if (current != null)
                            {
                                this.Cookies.Add(current);
                            }

                            current = new Cookie();
                            var idx = str.IndexOf('=');
                            if (idx > 0)
                            {
                                current.Name = str.Substring(0, idx).Trim();
                                current.Value = str.Substring(idx + 1).Trim();
                            }
                            else
                            {
                                current.Name = str.Trim();
                                current.Value = string.Empty;
                            }

                            current.Version = version;
                        }
                    }

                    if (current != null)
                    {
                        this.Cookies.Add(current);
                    }

                    break;
            }
        }

        internal void FinishInitialization()
        {
            var host = this.UserHostName;

            if (this.Version > HttpVersion.Http10 && string.IsNullOrEmpty(host))
            {
                this.context.ErrorMessage = "Invalid host name";
                return;
            }

            string path;

            Uri raw = null;

            if (UriExtensions.MaybeUri(this.RawUrl) && Uri.TryCreate(this.RawUrl, UriKind.Absolute, out raw))
            {
                path = raw.PathAndQuery;
            }
            else
            {
                path = this.RawUrl;
            }

            if (string.IsNullOrEmpty(host))
            {
                host = this.UserHostAddress;
            }

            if (raw != null)
            {
                host = raw.Host;
            }

            var colon = host.IndexOf(':');

            if (colon >= 0)
            {
                host = host.Substring(0, colon);
            }

            var baseUri = string.Format("{0}://{1}:{2}", this.IsSecureConnection ? "https" : "http", host, this.LocalEndPoint.Port);

            if (!Uri.TryCreate(baseUri + path, UriKind.Absolute, out this.url))
            {
                this.context.ErrorMessage = "Invalid url: " + baseUri + path;
                return;
            }

            this.CreateQueryString(this.url.Query);

            if (this.Version >= HttpVersion.Http11)
            {
                var enc = this.Headers["Transfer-Encoding"];

                var isChunked = enc != null && string.Compare(enc, "chunked", StringComparison.OrdinalIgnoreCase) == 0;

                // 'identity' is not valid!
                if (enc != null && !isChunked)
                {
                    this.context.Connection.SendError(null, 501);
                    return;
                }
            }

            //if (!is_chunked && !cl_set)
            //{
            //    if (String.Compare(method, "POST", StringComparison.OrdinalIgnoreCase) == 0 ||
            //        String.Compare(method, "PUT", StringComparison.OrdinalIgnoreCase) == 0)
            //    {
            //        context.Connection.SendError(null, 411);
            //        return;
            //    }
            //}

            //if (String.Compare(Headers["Expect"], "100-continue", StringComparison.OrdinalIgnoreCase) == 0)
            //{
            //    ResponseStream output = context.Connection.GetResponseStream();
            //    output.InternalWrite(_100continue, 0, _100continue.Length);
            //}
        }

        private static string Unquote(string str)
        {
            var start = str.IndexOf('\"');

            var end = str.LastIndexOf('\"');

            if (start >= 0 && end >= 0)
            {
                str = str.Substring(start + 1, end - 1);
            }

            return str.Trim();
        }

        private void CreateQueryString(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                this.QueryString = new NameValueCollection(1);
                return;
            }

            this.QueryString = new NameValueCollection();

            if (query[0] == '?')
            {
                query = query.Substring(1);
            }

            var components = query.Split('&');

            foreach (var kv in components)
            {
                int pos = kv.IndexOf('=');
                if (pos == -1)
                {
                    this.QueryString.Add(null, HttpUtility.UrlDecode(kv));
                }
                else
                {
                    var key = HttpUtility.UrlDecode(kv.Substring(0, pos));
                    var val = HttpUtility.UrlDecode(kv.Substring(pos + 1));

                    this.QueryString.Add(key, val);
                }
            }
        }
    }
}
