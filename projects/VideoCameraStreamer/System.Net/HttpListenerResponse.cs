using Windows.Storage.Streams;
using Windows.Web.Http;

namespace System.Net
{
    using System.Globalization;
    using System.IO;
    using System.Text;

    public sealed class HttpListenerResponse : IDisposable
    {
        private bool disposed;
        private Encoding contentEncoding;
        private long contentLength;
        private bool clSet;
        private string contentType;
        private CookieCollection cookies;
        private bool keepAlive = true;
        private IOutputStream outputStream;
        private HttpVersion version = HttpVersion.Http11;
        private string location;
        private int statusCode = 200;
        private string statusDescription = "OK";
        private bool chunked;
        private HttpListenerContext context;

        internal bool HeadersSent;

        internal object HeadersLock = new object();

        internal HttpListenerResponse(HttpListenerContext context)
        {
            this.context = context;
        }

        internal bool ForceCloseChunked { get; private set; }

        public Encoding ContentEncoding
        {
            get
            {
                return this.contentEncoding ?? (this.contentEncoding = Encoding.ASCII);
            }
            set
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException(GetType().ToString());
                }

                //TODO: is null ok?
                if (this.HeadersSent)
                {
                    throw new InvalidOperationException("Cannot be changed after headers are sent.");
                }

                this.contentEncoding = value;
            }
        }

        public long ContentLength64
        {
            get
            {
                return this.contentLength;
            }
            set
            {
                if (this.disposed)
                    throw new ObjectDisposedException(GetType().ToString());

                if (this.HeadersSent)
                    throw new InvalidOperationException("Cannot be changed after headers are sent.");

                if (value < 0)
                    throw new ArgumentOutOfRangeException("Must be >= 0", "value");

                this.clSet = true;
                this.contentLength = value;
            }
        }

        public string ContentType
        {
            get
            {
                return this.contentType;
            }
            set
            {
                // TODO: is null ok?
                if (this.disposed)
                    throw new ObjectDisposedException(GetType().ToString());

                if (this.HeadersSent)
                    throw new InvalidOperationException("Cannot be changed after headers are sent.");

                this.contentType = value;
            }
        }

        // RFC 2109, 2965 + the netscape specification at http://wp.netscape.com/newsref/std/cookie_spec.html
        public CookieCollection Cookies
        {
            get
            {
                if (cookies == null)
                {
                    cookies = new CookieCollection();
                }
                return cookies;
            }
            set { cookies = value; } // null allowed?
        }

        public WebHeaderCollection Headers { get; set; } = new WebHeaderCollection();

        public bool KeepAlive
        {
            get { return keepAlive; }
            set
            {
                if (disposed)
                    throw new ObjectDisposedException(GetType().ToString());

                if (HeadersSent)
                    throw new InvalidOperationException("Cannot be changed after headers are sent.");

                keepAlive = value;
            }
        }

        public IOutputStream OutputStream
        {
            get
            {
                if (outputStream == null)
                {
                    outputStream = context.Connection.GetResponseStream();
                }
                return outputStream;
            }
        }

        public HttpVersion ProtocolVersion
        {
            get
            {
                return version;
            }
            set
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(GetType().ToString());
                }

                if (HeadersSent)
                    throw new InvalidOperationException("Cannot be changed after headers are sent.");

                if (value == HttpVersion.Http10 || value == HttpVersion.Http11)
                {
                    throw new ArgumentException("Must be 1.0 or 1.1", "value");
                }

                if (disposed)
                {
                    throw new ObjectDisposedException(GetType().ToString());
                }

                version = value;
            }
        }

        public string RedirectLocation
        {
            get { return location; }
            set
            {
                if (disposed)
                    throw new ObjectDisposedException(GetType().ToString());

                if (HeadersSent)
                    throw new InvalidOperationException("Cannot be changed after headers are sent.");

                location = value;
            }
        }

        public bool SendChunked
        {
            get { return chunked; }
            set
            {
                if (disposed)
                    throw new ObjectDisposedException(GetType().ToString());

                if (HeadersSent)
                    throw new InvalidOperationException("Cannot be changed after headers are sent.");

                chunked = value;
            }
        }

        public int StatusCode
        {
            get { return statusCode; }
            set
            {
                if (disposed)
                    throw new ObjectDisposedException(GetType().ToString());

                if (HeadersSent)
                    throw new InvalidOperationException("Cannot be changed after headers are sent.");

                if (value < 100 || value > 999)
                    throw new ProtocolViolationException("StatusCode must be between 100 and 999.");
                statusCode = value;
                statusDescription = GetStatusDescription(value);
            }
        }

        internal static string GetStatusDescription(int code)
        {
            switch (code)
            {
                case 100: return "Continue";
                case 101: return "Switching Protocols";
                case 102: return "Processing";
                case 200: return "OK";
                case 201: return "Created";
                case 202: return "Accepted";
                case 203: return "Non-Authoritative Information";
                case 204: return "No Content";
                case 205: return "Reset Content";
                case 206: return "Partial Content";
                case 207: return "Multi-Status";
                case 300: return "Multiple Choices";
                case 301: return "Moved Permanently";
                case 302: return "Found";
                case 303: return "See Other";
                case 304: return "Not Modified";
                case 305: return "Use Proxy";
                case 307: return "Temporary Redirect";
                case 400: return "Bad Request";
                case 401: return "Unauthorized";
                case 402: return "Payment Required";
                case 403: return "Forbidden";
                case 404: return "Not Found";
                case 405: return "Method Not Allowed";
                case 406: return "Not Acceptable";
                case 407: return "Proxy Authentication Required";
                case 408: return "Request Timeout";
                case 409: return "Conflict";
                case 410: return "Gone";
                case 411: return "Length Required";
                case 412: return "Precondition Failed";
                case 413: return "Request Entity Too Large";
                case 414: return "Request-Uri Too Long";
                case 415: return "Unsupported Media Type";
                case 416: return "Requested Range Not Satisfiable";
                case 417: return "Expectation Failed";
                case 422: return "Unprocessable Entity";
                case 423: return "Locked";
                case 424: return "Failed Dependency";
                case 500: return "Internal Server Error";
                case 501: return "Not Implemented";
                case 502: return "Bad Gateway";
                case 503: return "Service Unavailable";
                case 504: return "Gateway Timeout";
                case 505: return "Http Version Not Supported";
                case 507: return "Insufficient Storage";
            }
            return "";
        }

        public string StatusDescription
        {
            get { return statusDescription; }
            set
            {
                statusDescription = value;
            }
        }

        void IDisposable.Dispose()
        {
            Close(true); //TODO: Abort or Close?
        }

        public void Abort()
        {
            if (disposed)
                return;

            Close(true);
        }

        public void AddHeader(string name, string value)
        {
            switch (name)
            {
                case null:
                    throw new ArgumentNullException(nameof(name));
                case "":
                    throw new ArgumentException("'name' cannot be empty", nameof(name));
            }

            //TODO: check for forbidden headers and invalid characters
            if (value.Length > 65535)
                throw new ArgumentOutOfRangeException(nameof(value));

            Headers[name] = value;
        }

        public void AppendCookie(Cookie cookie)
        {
            if (cookie == null)
                throw new ArgumentNullException("cookie");

            Cookies.Add(cookie);
        }

        public void AppendHeader(string name, string value)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name == "")
                throw new ArgumentException("'name' cannot be empty", "name");

            if (value.Length > 65535)
                throw new ArgumentOutOfRangeException("value");

            Headers[name] = value;
        }

        public void Close()
        {
            if (disposed)
                return;

            Close(false);
        }

        private void Close(bool force)
        {
            disposed = true;
            context.Connection.Close(force);
        }

        public void Close(byte[] responseEntity, bool willBlock)
        {
            if (disposed)
                return;

            if (responseEntity == null)
                throw new ArgumentNullException("responseEntity");

            //TODO: if willBlock -> BeginWrite + Close ?
            ContentLength64 = responseEntity.Length;
            OutputStream.Write(responseEntity, 0, (int)contentLength);
            Close(false);
        }

        public void CopyFrom(HttpListenerResponse templateResponse)
        {
            var keys = Headers.AllKeys;
            foreach (var key in keys)
            {
                Headers.Remove(key);
            }

            Headers = templateResponse.Headers;

            contentLength = templateResponse.contentLength;
            statusCode = templateResponse.statusCode;
            statusDescription = templateResponse.statusDescription;
            keepAlive = templateResponse.keepAlive;
            version = templateResponse.version;
        }

        public void Redirect(string url)
        {
            this.StatusCode = 302; // Found
            this.location = url;
        }

        public void SetCookie(Cookie cookie)
        {
            if (cookie == null)
            {
                throw new ArgumentNullException("cookie");
            }

            if (cookies != null)
            {
                if (FindCookie(cookie))
                {
                    throw new ArgumentException("The cookie already exists.");
                }
            }
            else
            {
                cookies = new CookieCollection();
            }

            cookies.Add(cookie);
        }

        private bool FindCookie(Cookie cookie)
        {
            var name = cookie.Name;
            var domain = cookie.Domain;
            var path = cookie.Path;

            foreach (Cookie c in this.cookies)
            {
                if (name != c.Name)
                {
                    continue;
                }
                if (domain != c.Domain)
                {
                    continue;
                }
                if (path == c.Path)
                {
                    return true;
                }
            }

            return false;
        }

        internal void SendHeaders(bool closing, MemoryStream ms)
        {
            var encoding = contentEncoding ?? Encoding.ASCII;

            if (contentType != null)
            {
                if (contentEncoding != null && contentType.IndexOf("charset=", StringComparison.Ordinal) == -1)
                {
                    var enc_name = contentEncoding.WebName;
                    this.Headers.SetInternal("Content-Type", contentType + "; charset=" + enc_name);
                }
                else
                {
                    this.Headers.SetInternal("Content-Type", contentType);
                }
            }

            if (this.Headers["Server"] == null)
            {
                this.Headers["Server"] = "UAP-HTTPAPI/1.0";
            }

            var inv = CultureInfo.InvariantCulture;
            if (this.Headers["Date"] == null)
            {
                this.Headers["Date"] = DateTime.UtcNow.ToString("r", inv);
            }

            if (!chunked)
            {
                if (!clSet && closing)
                {
                    clSet = true;
                    contentLength = 0;
                }

                if (clSet)
                {
                    this.Headers["Content-Length"] = contentLength.ToString(inv);
                }
            }

            var v = context.Request.ProtocolVersion;

            if (!clSet && !chunked && v >= HttpVersion.Http11)
            {
                chunked = true;
            }

            /* Apache forces closing the connection for these status codes:
			 *	HttpStatusCode.BadRequest 		400
			 *	HttpStatusCode.RequestTimeout 		408
			 *	HttpStatusCode.LengthRequired 		411
			 *	HttpStatusCode.RequestEntityTooLarge 	413
			 *	HttpStatusCode.RequestUriTooLong 	414
			 *	HttpStatusCode.InternalServerError 	500
			 *	HttpStatusCode.ServiceUnavailable 	503
			 */
            var conn_close = (statusCode == 400 || statusCode == 408 || statusCode == 411 ||
                    statusCode == 413 || statusCode == 414 || statusCode == 500 ||
                    statusCode == 503);

            if (conn_close == false)
                conn_close = !context.Request.KeepAlive;

            // They sent both KeepAlive: true and Connection: close!?
            if (!keepAlive || conn_close)
            {
                this.Headers["Connection"] = "close";
                conn_close = true;
            }

            if (chunked)
            {
                this.Headers["Transfer-Encoding"] = "chunked";
            }

            int reuses = context.Connection.Reuses;
            if (reuses >= 100)
            {
                ForceCloseChunked = true;
                if (!conn_close)
                {
                    this.Headers["Connection"] = "close";
                    conn_close = true;
                }
            }

            if (!conn_close)
            {
                Headers.SetInternal("Keep-Alive", String.Format("timeout=15,max={0}", 100 - reuses));
                if (context.Request.ProtocolVersion <= HttpVersion.Version10)
                    Headers.SetInternal("Connection", "keep-alive");
            }

            if (location != null)
                Headers.SetInternal("Location", location);

            if (cookies != null)
            {
                foreach (Cookie cookie in cookies)
                    Headers.SetInternal("Set-Cookie", cookie.ToClientString());
            }

            var writer = new StreamWriter(ms, encoding, 256);

            writer.Write("HTTP/{0} {1} {2}\r\n", version, statusCode, statusDescription);

            string headers_str = this.Headers.ToStringMultiValue();

            writer.Write(headers_str);
            writer.Flush();
            int preamble = (encoding.CodePage == 65001) ? 3 : encoding.GetPreamble().Length;

            if (this.outputStream == null)
            {
                this.outputStream = context.Connection.GetResponseStream();
            }

            /* Assumes that the ms was at position 0 */
            ms.Position = preamble;
            HeadersSent = true;
        }
    }
}