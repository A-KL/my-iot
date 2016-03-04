using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace System.Web.Http.Results
{
    public class OkResult : IHttpActionResult
    {
        private readonly IDependencyProvider dependencies;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Http.Results.BadRequestResult"/> class.
        /// </summary>
        /// <param name="controller">The controller from which to obtain the dependencies needed for execution.</param>
        public OkResult(ApiController controller)
          : this(new ApiControllerDependencyProvider(controller))
        {
        }

        private OkResult(IDependencyProvider dependencies)
        {
            this.dependencies = dependencies;
        }

        public HttpRequestMessage Requests
        {
            get
            {
                return this.dependencies.Request;
            }
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(StatusCodeResult.Execute(HttpStatusCode.OK, this.dependencies.Request));
        }
    }
}
