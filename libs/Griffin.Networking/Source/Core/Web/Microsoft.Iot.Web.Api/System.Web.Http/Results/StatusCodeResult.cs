using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace System.Web.Http.Results
{
    internal interface IDependencyProvider
    {
        HttpRequestMessage Request { get; }
    }

    internal sealed class ApiControllerDependencyProvider : IDependencyProvider
    {
        private readonly ApiController controller;
        private HttpRequestMessage request;

        public HttpRequestMessage Request
        {
            get
            {
                this.EnsureResolved();
                return this.request;
            }
        }

        public ApiControllerDependencyProvider(ApiController controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }
            this.controller = controller;
        }

        private void EnsureResolved()
        {
            if (this.request != null)
            {
                return;
            }

            var request = this.controller.Request;
            if (request == null)
            {
                throw new InvalidOperationException("SRResources.ApiController_RequestMustNotBeNull");
            }
            this.request = request;
        }
    }

    public class StatusCodeResult
    {
        private readonly HttpStatusCode statusCode;
        private readonly IDependencyProvider dependencies;

        /// <summary>
        /// Gets the HTTP status code for the response message.
        /// </summary>
        /// 
        /// <returns>
        /// The HTTP status code for the response message.
        /// </returns>
        public HttpStatusCode StatusCode
        {
            get
            {
                return this.statusCode;
            }
        }

        /// <summary>
        /// Gets the request message which led to this result.
        /// </summary>
        /// 
        /// <returns>
        /// The request message which led to this result.
        /// </returns>
        public HttpRequestMessage Request
        {
            get
            {
                return this.dependencies.Request;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Http.Results.StatusCodeResult"/> class.
        /// </summary>
        /// <param name="statusCode">The HTTP status code for the response message.</param><param name="controller">The controller from which to obtain the dependencies needed for execution.</param>
        public StatusCodeResult(HttpStatusCode statusCode, ApiController controller)
          : this(statusCode, new ApiControllerDependencyProvider(controller))
        {
        }

        private StatusCodeResult(HttpStatusCode statusCode, IDependencyProvider dependencies)
        {
            this.statusCode = statusCode;
            this.dependencies = dependencies;
        }

        /// <summary>
        /// Creates a response message asynchronously.
        /// </summary>
        /// 
        /// <returns>
        /// A task that, when completed, contains the response message.
        /// </returns>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        public virtual Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.Execute());
        }

        private HttpResponseMessage Execute()
        {
            return Execute(this.statusCode, this.dependencies.Request);
        }

        internal static HttpResponseMessage Execute(HttpStatusCode statusCode, HttpRequestMessage request)
        {
            var httpResponseMessage = new HttpResponseMessage(statusCode);
            try
            {
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
