using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace System.Web.Http.Results
{
    /// <summary>
    /// Represents an action result that returns formatted content.
    /// </summary>
    /// <typeparam name="T">The type of content in the entity body.</typeparam>
    public class FormattedContentResult<T> : IHttpActionResult
    {
        private readonly HttpStatusCode _statusCode;
        private readonly T _content;
        private readonly MediaTypeFormatter _formatter;
        private readonly MediaTypeHeaderValue _mediaType;
        private readonly StatusCodeResult.IDependencyProvider _dependencies;

        /// <summary>
        /// Gets the HTTP status code for the response message.
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get
            {
                return this._statusCode;
            }
        }

        /// <summary>
        /// Gets the content value to format in the entity body.
        /// </summary>
        public T Content
        {
            get
            {
                return this._content;
            }
        }

        /// <summary>
        /// Gets the formatter to use to format the content.
        /// </summary>
        public MediaTypeFormatter Formatter
        {
            get
            {
                return this._formatter;
            }
        }

        /// <summary>
        /// Gets the value for the Content-Type header, or <see cref="null"/> to have the formatter pick a default value.
        /// </summary>
        public MediaTypeHeaderValue MediaType
        {
            get
            {
                return this._mediaType;
            }
        }

        /// <summary>
        /// Gets the request message which led to this result.
        /// </summary>
        public HttpRequestMessage Request
        {
            get
            {
                return this._dependencies.Request;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Http.Results.FormattedContentResult`1"/> class with the values provided.
        /// </summary>
        /// <param name="statusCode">The HTTP status code for the response message.</param><param name="content">The content value to format in the entity body.</param><param name="formatter">The formatter to use to format the content.</param><param name="mediaType">The value for the Content-Type header, or <see cref="null"/> to have the formatter pick a default value.</param><param name="request">The request message which led to this result.</param>
        public FormattedContentResult(HttpStatusCode statusCode, T content, MediaTypeFormatter formatter, MediaTypeHeaderValue mediaType, HttpRequestMessage request)
          : this(statusCode, content, formatter, mediaType, (StatusCodeResult.IDependencyProvider)new StatusCodeResult.DirectDependencyProvider(request))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Http.Results.FormattedContentResult`1"/> class with the values provided.
        /// </summary>
        /// <param name="statusCode">The HTTP status code for the response message.</param><param name="content">The content value to format in the entity body.</param><param name="formatter">The formatter to use to format the content.</param><param name="mediaType">The value for the Content-Type header, or <see cref="null"/> to have the formatter pick a default value.</param><param name="controller">The controller from which to obtain the dependencies needed for execution.</param>
        public FormattedContentResult(HttpStatusCode statusCode, T content, MediaTypeFormatter formatter, MediaTypeHeaderValue mediaType, ApiController controller)
          : this(statusCode, content, formatter, mediaType, (StatusCodeResult.IDependencyProvider)new StatusCodeResult.ApiControllerDependencyProvider(controller))
        {
        }

        private FormattedContentResult(HttpStatusCode statusCode, T content, MediaTypeFormatter formatter, MediaTypeHeaderValue mediaType, StatusCodeResult.IDependencyProvider dependencies)
        {
            if (formatter == null)
                throw new ArgumentNullException("formatter");
            this._statusCode = statusCode;
            this._content = content;
            this._formatter = formatter;
            this._mediaType = mediaType;
            this._dependencies = dependencies;
        }

        public virtual Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<HttpResponseMessage>(this.Execute());
        }

        private HttpResponseMessage Execute()
        {
            return FormattedContentResult<T>.Execute(this._statusCode, this._content, this._formatter, this._mediaType, this._dependencies.Request);
        }

        internal static HttpResponseMessage Execute(HttpStatusCode statusCode, T content, MediaTypeFormatter formatter, MediaTypeHeaderValue mediaType, HttpRequestMessage request)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(statusCode);
            try
            {
                httpResponseMessage.Content = (HttpContent)new ObjectContent<T>(content, formatter, mediaType);
                httpResponseMessage.RequestMessage = request;
            }
            catch
            {
                httpResponseMessage.Dispose();
                throw;
            }
            return httpResponseMessage;
        }
    }
}
