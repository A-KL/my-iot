using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Results;

namespace System.Web.Http
{
    public abstract class ApiController : IHttpController, IDisposable
    {
        private bool _disposed;
        private HttpRequestMessage _request;
        //private ModelStateDictionary _modelState;
        private HttpConfiguration _configuration;
        private HttpControllerContext _controllerContext;
        // private UrlHelper _urlHelper;

        /// <summary>
        /// Gets the<see name="HttpRequestMessage"/>of the current ApiController.
        /// The setter is not intended to be used other than for unit testing purpose.
        /// </summary>
        public HttpRequestMessage Request
        {
            get { return _request; }
            set
            {
                if (value == null)
                {
                    throw Error.PropertyNull();
                }

                _request = value;
            }
        }

        /// <summary>
        /// Gets the<see name="HttpControllerContext"/>of the current ApiController.
        /// The setter is not intended to be used other than for unit testing purpose.
        /// </summary>
        public HttpControllerContext ControllerContext
        {
            get { return _controllerContext; }
            set
            {
                if (value == null)
                {
                    throw Error.PropertyNull();
                }

                _controllerContext = value;
            }
        }

        /// <summary>
        /// Gets the <see name="HttpConfiguration"/> of the current ApiController.
        /// 
        /// The setter is not intended to be used other than for unit testing purpose. 
        /// </summary>
        public HttpConfiguration Configuration
        {
            get { return _configuration; }
            set
            {
                if (value == null)
                {
                    throw Error.PropertyNull();
                }

                _configuration = value;
            }
        }

        public Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            if (_request != null)
            {
                // if user has registered a controller factory which produces the same controller instance, we should throw here
                throw Error.InvalidOperation("SRResources.CannotSupportSingletonInstance, typeof(ApiController).Name, typeof(IHttpControllerActivator).Name");
            }

            Initialize(controllerContext);

            throw new NotImplementedException();
        }

        protected virtual void Initialize(HttpControllerContext controllerContext)
        {
            if (controllerContext == null)
            {
                throw Error.ArgumentNull("controllerContext");
            }

            ControllerContext = controllerContext;

            _request = controllerContext.Request;
            _configuration = controllerContext.Configuration;
        }

        /// <summary>
        /// Creates a <see cref="T:System.Web.Http.Results.BadRequestResult"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Http.Results.BadRequestResult"/>.
        /// </returns>
        protected internal virtual BadRequestResult BadRequest()
        {
            return new BadRequestResult(this);
        }

        /// <summary>
        /// Creates an <see cref="T:System.Web.Http.Results.OkResult"/> (200 OK).
        /// </summary>        
        /// <returns>
        /// An <see cref="T:System.Web.Http.Results.OkResult"/>.
        /// </returns>
        protected internal virtual OkResult Ok()
        {
            return new OkResult(this);
        }

        /// <summary>
        /// Creates an <see cref="T:System.Web.Http.Results.OkNegotiatedContentResult`1"/> with the specified values.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Web.Http.Results.OkNegotiatedContentResult`1"/> with the specified values.
        /// </returns>
        /// <param name="content">The content value to negotiate and format in the entity body.</param><typeparam name="T">The type of content in the entity body.</typeparam>
        //protected internal virtual OkNegotiatedContentResult<T> Ok<T>(T content)
        //{
        //    return new OkNegotiatedContentResult<T>(content, this);
        //}

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                if (disposing)
                {
                    // TODO: Dispose controller state
                }
            }
        }

        #endregion
    }
}
