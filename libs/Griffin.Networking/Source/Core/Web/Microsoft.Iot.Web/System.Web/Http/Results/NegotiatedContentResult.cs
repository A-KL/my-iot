//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Formatting;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Web.Http;

//namespace System.Web.Http.Results
//{
//    /// <summary>
//    /// Represents an action result that performs content negotiation.
//    /// </summary>
//    /// <typeparam name="T">The type of content in the entity body.</typeparam>
//    public class NegotiatedContentResult<T> : IHttpActionResult
//    {
//        private readonly HttpStatusCode statusCode;
//        private readonly T content;
//        private readonly NegotiatedContentResult<T>.IDependencyProvider dependencies;

//        /// <summary>
//        /// Gets the HTTP status code for the response message.
//        /// </summary>
//        /// 
//        /// <returns>
//        /// The HTTP status code for the response message.
//        /// </returns>
//        public HttpStatusCode StatusCode
//        {
//            get
//            {
//                return this.statusCode;
//            }
//        }

//        /// <summary>
//        /// Gets the content value to negotiate and format in the entity body.
//        /// </summary>
//        /// 
//        /// <returns>
//        /// The content value to negotiate and format in the entity body.
//        /// </returns>
//        public T Content
//        {
//            get
//            {
//                return this.content;
//            }
//        }

//        /// <summary>
//        /// Gets the content negotiator to handle content negotiation.
//        /// </summary>
//        /// 
//        /// <returns>
//        /// The content negotiator to handle content negotiation.
//        /// </returns>
//        public IContentNegotiator ContentNegotiator
//        {
//            get
//            {
//                return this.dependencies.ContentNegotiator;
//            }
//        }

//        /// <summary>
//        /// Gets the request message which led to this result.
//        /// </summary>
//        /// 
//        /// <returns>
//        /// The HTTP request message which led to this result.
//        /// </returns>
//        public HttpRequestMessage Request
//        {
//            get
//            {
//                return this.dependencies.Request;
//            }
//        }

//        /// <summary>
//        /// Gets the formatters to use to negotiate and format the content.
//        /// </summary>
//        /// 
//        /// <returns>
//        /// The formatters to use to negotiate and format the content.
//        /// </returns>
//        public IEnumerable<MediaTypeFormatter> Formatters
//        {
//            get
//            {
//                return this.dependencies.Formatters;
//            }
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="T:System.Web.Http.Results.NegotiatedContentResult`1"/> class with the values provided.
//        /// </summary>
//        /// <param name="statusCode">The HTTP status code for the response message.</param><param name="content">The content value to negotiate and format in the entity body.</param><param name="contentNegotiator">The content negotiator to handle content negotiation.</param><param name="request">The request message which led to this result.</param><param name="formatters">The formatters to use to negotiate and format the content.</param>
//        public NegotiatedContentResult(HttpStatusCode statusCode, T content, IContentNegotiator contentNegotiator, HttpRequestMessage request, IEnumerable<MediaTypeFormatter> formatters)
//          : this(statusCode, content, (NegotiatedContentResult<T>.IDependencyProvider)new NegotiatedContentResult<T>.DirectDependencyProvider(contentNegotiator, request, formatters))
//        {
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="T:System.Web.Http.Results.NegotiatedContentResult`1"/> class with the values provided.
//        /// </summary>
//        /// <param name="statusCode">The HTTP status code for the response message.</param><param name="content">The content value to negotiate and format in the entity body.</param><param name="controller">The controller from which to obtain the dependencies needed for execution.</param>
//        public NegotiatedContentResult(HttpStatusCode statusCode, T content, ApiController controller)
//          : this(statusCode, content, (NegotiatedContentResult<T>.IDependencyProvider)new NegotiatedContentResult<T>.ApiControllerDependencyProvider(controller))
//        {
//        }

//        private NegotiatedContentResult(HttpStatusCode statusCode, T content, NegotiatedContentResult<T>.IDependencyProvider dependencies)
//        {
//            this.statusCode = statusCode;
//            this.content = content;
//            this.dependencies = dependencies;
//        }

//        /// <summary>
//        /// Executes asynchronously an HTTP negotiated content results.
//        /// </summary>
//        /// 
//        /// <returns>
//        /// Asynchronously executes an HTTP negotiated content results.
//        /// </returns>
//        /// <param name="cancellationToken">The cancellation token.</param>
//        public virtual Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
//        {
//            return Task.FromResult<HttpResponseMessage>(this.Execute());
//        }

//        private HttpResponseMessage Execute()
//        {
//            return NegotiatedContentResult<T>.Execute(this.statusCode, this.content, this.dependencies.ContentNegotiator, this.dependencies.Request, this.dependencies.Formatters);
//        }

//        internal static HttpResponseMessage Execute(HttpStatusCode statusCode, T content, IContentNegotiator contentNegotiator, HttpRequestMessage request, IEnumerable<MediaTypeFormatter> formatters)
//        {
//            var negotiationResult = contentNegotiator.Negotiate(typeof(T), request, formatters);
//            var httpResponseMessage = new HttpResponseMessage();

//            try
//            {
//                if (negotiationResult == null)
//                {
//                    httpResponseMessage.StatusCode = HttpStatusCode.NotAcceptable;
//                }
//                else
//                {
//                    httpResponseMessage.StatusCode = statusCode;
//                    httpResponseMessage.Content = (HttpContent)new ObjectContent<T>(content, negotiationResult.Formatter, negotiationResult.MediaType);
//                }
//                httpResponseMessage.RequestMessage = request;
//            }
//            catch
//            {
//                httpResponseMessage.Dispose();
//                throw;
//            }
//            return httpResponseMessage;
//        }

//        internal interface IDependencyProvider
//        {
//            IContentNegotiator ContentNegotiator { get; }

//            HttpRequestMessage Request { get; }

//            IEnumerable<MediaTypeFormatter> Formatters { get; }
//        }

//        internal sealed class DirectDependencyProvider : NegotiatedContentResult<T>.IDependencyProvider
//        {
//            private readonly IContentNegotiator contentNegotiator;
//            private readonly HttpRequestMessage request;
//            private readonly IEnumerable<MediaTypeFormatter> formatters;

//            public IContentNegotiator ContentNegotiator
//            {
//                get
//                {
//                    return this.contentNegotiator;
//                }
//            }

//            public HttpRequestMessage Request
//            {
//                get
//                {
//                    return this.request;
//                }
//            }

//            public IEnumerable<MediaTypeFormatter> Formatters
//            {
//                get
//                {
//                    return this.formatters;
//                }
//            }

//            public DirectDependencyProvider(IContentNegotiator contentNegotiator, HttpRequestMessage request, IEnumerable<MediaTypeFormatter> formatters)
//            {
//                if (contentNegotiator == null)
//                    throw new ArgumentNullException("contentNegotiator");
//                if (request == null)
//                    throw new ArgumentNullException("request");
//                if (formatters == null)
//                    throw new ArgumentNullException("formatters");
//                this.contentNegotiator = contentNegotiator;
//                this.request = request;
//                this.formatters = formatters;
//            }
//        }

//        internal sealed class ApiControllerDependencyProvider : NegotiatedContentResult<T>.IDependencyProvider
//        {
//            private readonly ApiController controller;
//            private NegotiatedContentResult<T>.IDependencyProvider resolvedDependencies;

//            public IContentNegotiator ContentNegotiator
//            {
//                get
//                {
//                    this.EnsureResolved();
//                    return this.resolvedDependencies.ContentNegotiator;
//                }
//            }

//            public HttpRequestMessage Request
//            {
//                get
//                {
//                    this.EnsureResolved();
//                    return this.resolvedDependencies.Request;
//                }
//            }

//            public IEnumerable<MediaTypeFormatter> Formatters
//            {
//                get
//                {
//                    this.EnsureResolved();
//                    return this.resolvedDependencies.Formatters;
//                }
//            }

//            public ApiControllerDependencyProvider(ApiController controller)
//            {
//                if (controller == null)
//                    throw new ArgumentNullException("controller");
//                this.controller = controller;
//            }

//            private void EnsureResolved()
//            {
//                if (this.resolvedDependencies != null)
//                {
//                    return;
//                }

//                HttpConfiguration configuration = this.controller.Configuration;

//                if (configuration == null)
//                {
//                    throw new InvalidOperationException(SRResources.HttpControllerContext_ConfigurationMustNotBeNull);
//                }

//                IContentNegotiator contentNegotiator = ServicesExtensions.GetContentNegotiator(configuration.Services);

//                if (contentNegotiator == null)
//                {
//                    throw new InvalidOperationException(Error.Format(SRResources.HttpRequestMessageExtensions_NoContentNegotiator, (object)typeof(IContentNegotiator)));
//                }
//                HttpRequestMessage request = this.controller.Request;
//                if (request == null)
//                {
//                    throw new InvalidOperationException(SRResources.ApiController_RequestMustNotBeNull);
//                }
//                IEnumerable<MediaTypeFormatter> formatters = (IEnumerable<MediaTypeFormatter>)configuration.Formatters;
//                this.resolvedDependencies = (NegotiatedContentResult<T>.IDependencyProvider)new NegotiatedContentResult<T>.DirectDependencyProvider(contentNegotiator, request, formatters);
//            }
//        }
//    }
//}
