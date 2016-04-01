// Decompiled with JetBrains decompiler
// Type: System.Web.Http.Results.StatusCodeResult
// Assembly: System.Web.Http, Version=5.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: F99F496C-B0D2-49C1-A945-C1FCABCE1695
// Assembly location: E:\Assembla-SVN\3DProjects\MyProjects\Windows\WebStreamingService\WebStreamingService\Bin\System.Web.Http.dll

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Properties;

namespace System.Web.Http.Results
{
    /// <summary>
    /// Represents an action result that returns a specified HTTP status code.
    /// </summary>
    public class StatusCodeResult : IHttpActionResult
    {
        private readonly HttpStatusCode _statusCode;
        private readonly StatusCodeResult.IDependencyProvider _dependencies;

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
                return this._statusCode;
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
                return this._dependencies.Request;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Http.Results.StatusCodeResult"/> class.
        /// </summary>
        /// <param name="statusCode">The HTTP status code for the response message.</param><param name="request">The request message which led to this result.</param>
        public StatusCodeResult(HttpStatusCode statusCode, HttpRequestMessage request)
          : this(statusCode, (StatusCodeResult.IDependencyProvider)new StatusCodeResult.DirectDependencyProvider(request))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Http.Results.StatusCodeResult"/> class.
        /// </summary>
        /// <param name="statusCode">The HTTP status code for the response message.</param><param name="controller">The controller from which to obtain the dependencies needed for execution.</param>
        public StatusCodeResult(HttpStatusCode statusCode, ApiController controller)
          : this(statusCode, (StatusCodeResult.IDependencyProvider)new StatusCodeResult.ApiControllerDependencyProvider(controller))
        {
        }

        private StatusCodeResult(HttpStatusCode statusCode, StatusCodeResult.IDependencyProvider dependencies)
        {
            this._statusCode = statusCode;
            this._dependencies = dependencies;
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
            return Task.FromResult<HttpResponseMessage>(this.Execute());
        }

        private HttpResponseMessage Execute()
        {
            return StatusCodeResult.Execute(this._statusCode, this._dependencies.Request);
        }

        internal static HttpResponseMessage Execute(HttpStatusCode statusCode, HttpRequestMessage request)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(statusCode);
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

        internal interface IDependencyProvider
        {
            HttpRequestMessage Request { get; }
        }

        internal sealed class DirectDependencyProvider : StatusCodeResult.IDependencyProvider
        {
            private readonly HttpRequestMessage _request;

            public HttpRequestMessage Request
            {
                get
                {
                    return this._request;
                }
            }

            public DirectDependencyProvider(HttpRequestMessage request)
            {
                if (request == null)
                    throw new ArgumentNullException("request");
                this._request = request;
            }
        }

        internal sealed class ApiControllerDependencyProvider : StatusCodeResult.IDependencyProvider
        {
            private readonly ApiController _controller;
            private HttpRequestMessage _request;

            public HttpRequestMessage Request
            {
                get
                {
                    this.EnsureResolved();
                    return this._request;
                }
            }

            public ApiControllerDependencyProvider(ApiController controller)
            {
                if (controller == null)
                    throw new ArgumentNullException("controller");
                this._controller = controller;
            }

            private void EnsureResolved()
            {
                if (this._request != null)
                    return;
                HttpRequestMessage request = this._controller.Request;
                if (request == null)
                    throw new InvalidOperationException("SRResources.ApiController_RequestMustNotBeNull");
                this._request = request;
            }
        }
    }
}
