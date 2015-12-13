namespace Windows.Http
{
    using global::System;
    using global::System.Net;

    using Storage.Streams;

    using Web.Http;

    public sealed class HttpListenerRequest
    {
        private WebHeaderCollection headers;

        private string path;

        private IOutputStream outputStream;

        private readonly HttpListenerContext context;


        public HttpListenerRequest(HttpListenerContext context)
        {
            this.context = context;
        }

        public WebHeaderCollection Headers
        {
            get { return this.headers; }
        }

        public Uri Url
        {
            get; private set;
        }

        public string HttpMethod
        {
            get; private set;
        }

        public HttpVersion Version
        {
            get; private set;
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
    }
}
