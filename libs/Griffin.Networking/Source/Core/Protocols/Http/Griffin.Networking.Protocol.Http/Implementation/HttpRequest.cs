﻿using System;
using System.IO;
using System.Net;
using System.Text;
using Griffin.Networking.Protocol.Http.Implementation.Infrastructure;
using Griffin.Networking.Protocol.Http.Protocol;

namespace Griffin.Networking.Protocol.Http.Implementation
{
    /// <summary>
    /// HTTTP request implementation
    /// </summary>
    public class HttpRequest : HttpMessage, IRequest
    {
        private IHttpCookieCollection<IHttpCookie> cookies;
        private readonly IHttpFileCollection files;
        private readonly IParameterCollection form;
        private readonly string pathAndQuery;
        private readonly ParameterCollection queryString;
        private Uri uri;

        private const string WebSocketSecKeyHeader = "Sec-WebSocket-Key";
        private const string UpgradeHeader = "Upgrade";
        private const string ConnectionHeader = "Connection";

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequest" /> class.
        /// </summary>
        public HttpRequest()
        {
            cookies = new HttpCookieCollection<IHttpCookie>();
            files = new HttpFileCollection();
            queryString = new ParameterCollection();
            form = new ParameterCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequest" /> class.
        /// </summary>
        /// <param name="httpMethod">The HTTP method like "POST" or "GET".</param>
        /// <param name="url">The url path including query string.</param>
        /// <param name="httpVersion">The HTTP version. Typically "HTTP/1.1"</param>
        /// <exception cref="System.ArgumentNullException">httpMethod</exception>
        public HttpRequest(string httpMethod, string url, string httpVersion)
            : this()
        {
            if (httpMethod == null) throw new ArgumentNullException("httpMethod");
            if (url == null) throw new ArgumentNullException("url");
            if (httpVersion == null) throw new ArgumentNullException("httpVersion");
            Method = httpMethod;

            Uri = ProduceGoodUri(url);
            pathAndQuery = Uri.PathAndQuery;

            ProtocolVersion = httpVersion;
        }

        #region IRequest Members

        /// <summary>
        /// Gets or sets if connection is being kept alive
        /// </summary>
        public bool KeepAlive
        {
            get
            {
                var header = Headers["Connection"];
                if (header == null || string.IsNullOrEmpty(header.Value))
                    return false;

                return header.Value.Equals("Keep-Alive", StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Gets content type
        /// </summary>
        /// <remarks>Any extra parameters are stripped. Use <see cref="Headers"/> to get the raw value</remarks>
        public string ContentType
        {
            get
            {
                var header = Headers["Content-Type"];
                return header != null ? header.Value : null;
            }
        }

        /// <summary>
        /// Gets cookies.
        /// </summary>
        public IHttpCookieCollection<IHttpCookie> Cookies
        {
            get { return cookies; }
        }

        /// <summary>
        /// Gets all uploaded files.
        /// </summary>
        public IHttpFileCollection Files
        {
            get { return files; }
        }

        /// <summary>
        /// Gets form parameters.
        /// </summary>
        public IParameterCollection Form
        {
            get { return form; }
        }

        /// <summary>
        /// Gets where the request originated from.
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; set; }

        /// <summary>
        /// Gets if request is an Ajax request.
        /// </summary>
        public bool IsAjax
        {
            get
            {
                var header = Headers["X-Requested-Width"];
                if (header == null || string.IsNullOrEmpty(header.Value))
                    return false;

                return header.Value.Equals("Ajax", StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Gets if request is an WebSocket connection request.
        /// </summary>
        public bool IsWebSocketRequest
        {
            get
            {
                var header = this.Headers[WebSocketSecKeyHeader];
                if (header == null || string.IsNullOrEmpty(header.Value))
                {
                    return false;
                }

                header = this.Headers[ConnectionHeader];
                if (header == null || string.IsNullOrEmpty(header.Value) || header.Value.Equals("Upgrade", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                header = this.Headers[UpgradeHeader];
                if (header == null || string.IsNullOrEmpty(header.Value) || header.Value.Equals("websocket", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets or sets HTTP method.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets query string.
        /// </summary>
        public IParameterCollection QueryString
        {
            get { return queryString; }
        }

        /// <summary>
        /// Gets requested URI.
        /// </summary>
        public Uri Uri
        {
            get { return uri; }
            set
            {
                uri = value;
                var decoder = new UrlDecoder();
                queryString.Clear();
                using (var reader = new StringReader(value.Query.TrimStart('?')))
                {
                    decoder.Parse(reader, QueryString);
                }
            }
        }

        /// <summary>
        /// Create a response for the request.
        /// </summary>
        /// <param name="code">Status code</param>
        /// <param name="reason">Gives the remote end point a hint to why the specified status code as used.</param>
        /// <returns>Created response</returns>
        /// <remarks>Can be used by implementations to transfer context specific information. It's prefered that you use this method
        /// instead of instantianting a response directly.</remarks>
        public IResponse CreateResponse(HttpStatusCode code, string reason)
        {
            if (reason == null) throw new ArgumentNullException("reason");
            return new HttpResponse(ProtocolVersion, code, reason);
        }

        /// <summary>
        /// Add a new header
        /// </summary>
        /// <param name="name">Name of the header</param>
        /// <param name="value">Value</param>
        /// <remarks>
        /// Adding a header which already exists will just append the value to that header.
        /// </remarks>
        public override void AddHeader(string name, string value)
        {
            if (name.Equals("host", StringComparison.OrdinalIgnoreCase))
            {
                Uri = value.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                          ? new Uri(string.Format("{0}{1}", value, pathAndQuery))
                          : new Uri(string.Format("http://{0}{1}", value, pathAndQuery));
            }
            if (name.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
            {
                ParseContentType(value);
                return;
            }
            if (name.Equals("Content-Length", StringComparison.CurrentCultureIgnoreCase))
            {
                ContentLength = int.Parse(value);
            }
            if (name.Equals("Cookie", StringComparison.OrdinalIgnoreCase))
            {
                var parser = new HttpCookieParser(value);
                cookies = parser.Parse();
            }

            base.AddHeader(name, value);
        }

        private void ParseContentType(string value)
        {
            var charsetPos = value.IndexOf(';');
            if (charsetPos != -1)
            {
                var encoding = value.Substring(charsetPos + 1).Trim();

                //TODO: Add a more solid implementation
                // which can handle all parameter types
                if (encoding.StartsWith("charset="))
                {
                    encoding = encoding.Remove(0, "charset=".Length);
                    try
                    {
                        ContentEncoding = Encoding.GetEncoding(encoding);
                    }
                    catch (Exception err)
                    {
                        throw new BadRequestException("Failed to load encoding '" +
                                                      encoding + "'.", err);
                    }
                }

                value = value.Substring(0, charsetPos);
            }

            base.AddHeader("Content-Type", value);
        }

        #endregion

        #region Methods / Static

        /// <summary>
        /// Generate a full url.
        /// If we only get the path a dummy scheme and domain will be used.
        /// The host will later be replaced when parsin the HTTP Host header.
        /// </summary>
        private static Uri ProduceGoodUri(string urlString)
        {
            Uri uri;
            if (Uri.TryCreate(urlString, UriKind.RelativeOrAbsolute, out uri))
            {
                if (uri.IsAbsoluteUri)
                    return uri;
            }
            //"invalid.host" will be replaced later when the "Host" header is parsed.
            return new Uri("http://invalid.host" + urlString);
        }

        #endregion
    }
}