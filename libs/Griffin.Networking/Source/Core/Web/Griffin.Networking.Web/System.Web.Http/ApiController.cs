namespace System.Web.Http
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Controllers;
    using System.Web.Http.Results;

    public abstract class ApiController : IHttpController, IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a <see cref="T:System.Web.Http.Results.BadRequestResult"/>.
        /// </summary>
        /// 
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
        /// 
        /// <returns>
        /// An <see cref="T:System.Web.Http.Results.OkResult"/>.
        /// </returns>
        protected internal virtual OkResult Ok()
        {
            return new OkResult(this);
        }
        
        /// <summary>
        /// Gets or sets the HttpRequestMessage of the current <see cref="T:System.Web.Http.ApiController"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The HttpRequestMessage of the current <see cref="T:System.Web.Http.ApiController"/>.
        /// </returns>
        public HttpRequestMessage Request
        {
            get; set;
        }

        /// <summary>
        /// Creates an <see cref="T:System.Web.Http.Results.OkNegotiatedContentResult`1"/> with the specified values.
        /// </summary>
        /// 
        /// <returns>
        /// An <see cref="T:System.Web.Http.Results.OkNegotiatedContentResult`1"/> with the specified values.
        /// </returns>
        /// <param name="content">The content value to negotiate and format in the entity body.</param><typeparam name="T">The type of content in the entity body.</typeparam>
        //protected internal virtual OkNegotiatedContentResult<T> Ok<T>(T content)
        //{
        //    return new OkNegotiatedContentResult<T>(content, this);
        //}
    }
}
