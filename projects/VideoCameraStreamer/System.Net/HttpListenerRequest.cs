using Windows.Web.Http;

namespace System.Net
{
    using System.Collections.Specialized;
    using System.IO;
    using System.Text;
    using System.Xml.Linq;

    public sealed class HttpListenerRequest
    {
        private HttpListenerContext context;

        private string[] accept_types;
        private Encoding content_encoding;
        private long content_length;
        private bool cl_set;
        private CookieCollection cookies;
        private WebHeaderCollection headers;
        private string method;
        private Stream input_stream;
        private Uri url;

        private bool is_chunked;
        private bool ka_set;
        private bool keep_alive;

        private static char[] separators = { ' ' };

        internal HttpListenerRequest(HttpListenerContext context)
        {
            this.context = context;
            this.headers = new WebHeaderCollection();
            this.ProtocolVersion = HttpVersion.Http10;
        }

        internal void SetRequestLine(string req)
        {
            var parts = req.Split(separators, 3);

            if (parts.Length != 3)
            {
                context.ErrorMessage = "Invalid request line (parts).";
                return;
            }

            method = parts[0];
            foreach (char c in method)
            {
                int ic = (int)c;

                if ((ic >= 'A' && ic <= 'Z') ||
                    (ic > 32 && c < 127 && c != '(' && c != ')' && c != '<' &&
                     c != '<' && c != '>' && c != '@' && c != ',' && c != ';' &&
                     c != ':' && c != '\\' && c != '"' && c != '/' && c != '[' &&
                     c != ']' && c != '?' && c != '=' && c != '{' && c != '}'))
                    continue;

                context.ErrorMessage = "(Invalid verb)";
                return;
            }

            RawUrl = parts[1];
            if (parts[2].Length != 8 || !parts[2].StartsWith("HTTP/"))
            {
                context.ErrorMessage = "Invalid request line (version).";
                return;
            }

            try
            {
                ProtocolVersion = new Version(parts[2].Substring(5));
                if (ProtocolVersion.Major < 1)
                    throw new Exception();
            }
            catch
            {
                context.ErrorMessage = "Invalid request line (version).";
                return;
            }
        }

        private void CreateQueryString(string query)
        {
            if (query == null || query.Length == 0)
            {
                QueryString = new NameValueCollection(1);
                return;
            }

            QueryString = new NameValueCollection();
            if (query[0] == '?')
                query = query.Substring(1);
            string[] components = query.Split('&');
            foreach (string kv in components)
            {
                int pos = kv.IndexOf('=');
                if (pos == -1)
                {
                    QueryString.Add(null, HttpUtility.UrlDecode(kv));
                }
                else
                {
                    string key = HttpUtility.UrlDecode(kv.Substring(0, pos));
                    string val = HttpUtility.UrlDecode(kv.Substring(pos + 1));

                    QueryString.Add(key, val);
                }
            }
        }

        internal void FinishInitialization()
        {
            string host = UserHostName;
            if (ProtocolVersion > HttpVersion.Version10 && (host == null || host.Length == 0))
            {
                context.ErrorMessage = "Invalid host name";
                return;
            }

            string path;
            Uri raw_uri = null;
            if (Extensions.MaybeUri(RawUrl) && Uri.TryCreate(RawUrl, UriKind.Absolute, out raw_uri))
                path = raw_uri.PathAndQuery;
            else
                path = RawUrl;

            if ((host == null || host.Length == 0))
                host = UserHostAddress;

            if (raw_uri != null)
                host = raw_uri.Host;

            int colon = host.IndexOf(':');
            if (colon >= 0)
                host = host.Substring(0, colon);

            string base_uri = String.Format("{0}://{1}:{2}",
                                (IsSecureConnection) ? "https" : "http",
                                host, LocalEndPoint.Port);

            if (!Uri.TryCreate(base_uri + path, UriKind.Absolute, out url))
            {
                context.ErrorMessage = "Invalid url: " + base_uri + path;
                return;
            }

            CreateQueryString(url.Query);

            if (ProtocolVersion >= HttpVersion.Version11)
            {
                string t_encoding = Headers["Transfer-Encoding"];
                is_chunked = (t_encoding != null && String.Compare(t_encoding, "chunked", StringComparison.OrdinalIgnoreCase) == 0);
                // 'identity' is not valid!
                if (t_encoding != null && !is_chunked)
                {
                    context.Connection.SendError(null, 501);
                    return;
                }
            }

            if (!is_chunked && !cl_set)
            {
                if (String.Compare(method, "POST", StringComparison.OrdinalIgnoreCase) == 0 ||
                    String.Compare(method, "PUT", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    context.Connection.SendError(null, 411);
                    return;
                }
            }

            if (String.Compare(Headers["Expect"], "100-continue", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ResponseStream output = context.Connection.GetResponseStream();
                output.InternalWrite(_100continue, 0, _100continue.Length);
            }
        }

        internal static string Unquote(string str)
        {
            var start = str.IndexOf('\"');
            var end = str.LastIndexOf('\"');
            if (start >= 0 && end >= 0)
            {
                str = str.Substring(start + 1, end - 1);
            }
            return str.Trim();
        }

        internal void AddHeader(string header)
        {
            var colon = header.IndexOf(':');
            if (colon == -1 || colon == 0)
            {
                context.ErrorMessage = "Bad Request";
                context.ErrorStatus = 400;
                return;
            }

            var name = header.Substring(0, colon).Trim();
            var val = header.Substring(colon + 1).Trim();
            var lower = name.ToLower();

            headers.SetInternal(name, val);
            switch (lower)
            {
                case "accept-language":
                    UserLanguages = val.Split(','); // yes, only split with a ','
                    break;
                case "accept":
                    accept_types = val.Split(','); // yes, only split with a ','
                    break;
                case "content-length":
                    try
                    {
                        //TODO: max. content_length?
                        content_length = Int64.Parse(val.Trim());
                        if (content_length < 0)
                            context.ErrorMessage = "Invalid Content-Length.";
                        cl_set = true;
                    }
                    catch
                    {
                        context.ErrorMessage = "Invalid Content-Length.";
                    }

                    break;
                case "referer":
                    try
                    {
                        UrlReferrer = new Uri(val);
                    }
                    catch
                    {
                        UrlReferrer = new Uri("http://someone.is.screwing.with.the.headers.com/");
                    }
                    break;
                case "cookie":
                    if (cookies == null)
                        cookies = new CookieCollection();

                    string[] cookieStrings = val.Split(new char[] { ',', ';' });
                    Cookie current = null;
                    int version = 0;
                    foreach (string cookieString in cookieStrings)
                    {
                        string str = cookieString.Trim();
                        if (str.Length == 0)
                            continue;
                        if (str.StartsWith("$Version"))
                        {
                            version = Int32.Parse(Unquote(str.Substring(str.IndexOf('=') + 1)));
                        }
                        else if (str.StartsWith("$Path"))
                        {
                            if (current != null)
                                current.Path = str.Substring(str.IndexOf('=') + 1).Trim();
                        }
                        else if (str.StartsWith("$Domain"))
                        {
                            if (current != null)
                                current.Domain = str.Substring(str.IndexOf('=') + 1).Trim();
                        }
                        else if (str.StartsWith("$Port"))
                        {
                            if (current != null)
                                current.Port = str.Substring(str.IndexOf('=') + 1).Trim();
                        }
                        else
                        {
                            if (current != null)
                            {
                                cookies.Add(current);
                            }
                            current = new Cookie();
                            int idx = str.IndexOf('=');
                            if (idx > 0)
                            {
                                current.Name = str.Substring(0, idx).Trim();
                                current.Value = str.Substring(idx + 1).Trim();
                            }
                            else
                            {
                                current.Name = str.Trim();
                                current.Value = String.Empty;
                            }
                            current.Version = version;
                        }
                    }
                    if (current != null)
                    {
                        cookies.Add(current);
                    }
                    break;
            }
        }

        // returns true is the stream could be reused.
        internal bool FlushInput()
        {
            if (!HasEntityBody)
                return true;

            int length = 2048;
            if (content_length > 0)
                length = (int)System.Math.Min(content_length, (long)length);

            byte[] bytes = new byte[length];
            while (true)
            {
                // TODO: test if MS has a timeout when doing this
                try
                {
                    IAsyncResult ares = InputStream.BeginRead(bytes, 0, length, null, null);
                    if (!ares.IsCompleted && !ares.AsyncWaitHandle.WaitOne(1000))
                        return false;
                    if (InputStream.EndRead(ares) <= 0)
                        return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public string[] AcceptTypes
        {
            get { return accept_types; }
        }

        public Encoding ContentEncoding
        {
            get
            {
                return content_encoding ?? (content_encoding = Encoding.ASCII);
            }
        }

        public long ContentLength64
        {
            get { return content_length; }
        }

        public string ContentType
        {
            get { return headers["content-type"]; }
        }

        public CookieCollection Cookies
        {
            get
            {
                // TODO: check if the collection is read-only
                if (cookies == null)
                    cookies = new CookieCollection();
                return cookies;
            }
        }

        public bool HasEntityBody
        {
            get { return (content_length > 0 || is_chunked); }
        }

        public NameValueCollection Headers
        {
            get { return headers; }
        }

        public string HttpMethod
        {
            get { return method; }
        }

        public Stream InputStream
        {
            get
            {
                if (this.input_stream == null)
                {
                    if (this.is_chunked || this.content_length > 0)
                    {
                        this.input_stream = this.context.Connection.GetRequestStream(this.is_chunked, this.content_length);
                    }
                    else
                    {
                        this.input_stream = Stream.Null;
                    }
                }

                return this.input_stream;
            }
        }

        public bool IsAuthenticated
        {
            get { return false; }
        }

        public bool IsLocal
        {
            get { return IPAddress.IsLoopback(this.RemoteEndPoint.Address); }
        }

        public bool KeepAlive
        {
            get
            {
                if (this.ka_set)
                {
                    return this.keep_alive;
                }

                this.ka_set = true;

                // 1. Connection header
                // 2. Protocol (1.1 == keep-alive by default)
                // 3. Keep-Alive header

                var cnc = headers["Connection"];
                if (!string.IsNullOrEmpty(cnc))
                {
                    keep_alive = (0 == string.Compare(cnc, "keep-alive", StringComparison.OrdinalIgnoreCase));
                }
                else if (ProtocolVersion == HttpVersion.Http10)
                {
                    keep_alive = true;
                }
                else
                {
                    cnc = headers["keep-alive"];

                    if (!string.IsNullOrEmpty(cnc))
                    {
                        keep_alive = (0 != string.Compare(cnc, "closed", StringComparison.OrdinalIgnoreCase));
                    }
                }
                return keep_alive;
            }
        }

        public IPEndPoint LocalEndPoint
        {
            get { return context.Connection.LocalEndPoint; }
        }

        public HttpVersion ProtocolVersion { get; private set; }

        public NameValueCollection QueryString { get; private set; }

        public string RawUrl { get; private set; }

        public IPEndPoint RemoteEndPoint
        {
            get { return context.Connection.RemoteEndPoint; }
        }

        public Uri Url
        {
            get { return url; }
        }

        public Uri UrlReferrer { get; private set; }

        public string UserAgent
        {
            get { return headers["user-agent"]; }
        }

        public string UserHostAddress
        {
            get { return LocalEndPoint.ToString(); }
        }

        public string UserHostName
        {
            get { return headers["host"]; }
        }

        public string[] UserLanguages { get; private set; }

        public bool IsWebSocketRequest
        {
            get { return false; }
        }
    }
}