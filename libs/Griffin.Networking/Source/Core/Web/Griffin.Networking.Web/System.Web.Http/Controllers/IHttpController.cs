namespace System.Web.Http.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an HTTP controller.
    /// </summary>
    public interface IHttpController
    {
        /// <summary>
        /// Executes the controller for synchronization.
        /// </summary>
        /// 
        /// <returns>
        /// The controller.
        /// </returns>
        /// <param name="controllerContext">The current context for a test controller.</param><param name="cancellationToken">The notification that cancels the operation.</param>
        Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken);
    }
}
