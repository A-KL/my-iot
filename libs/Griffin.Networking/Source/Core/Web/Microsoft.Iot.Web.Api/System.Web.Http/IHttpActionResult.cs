using System.Threading;
using System.Threading.Tasks;

namespace System.Web.Http
{
    /// <summary>
    /// Defines a command that asynchronously creates an <see cref="T:System.Net.Http.HttpResponseMessage"/>.
    /// </summary>
    public interface IHttpActionResult
    {
        /// <summary>
        /// Creates an <see cref="T:System.Net.Http.HttpResponseMessage"/> asynchronously.
        /// </summary>
        /// 
        /// <returns>
        /// A task that, when completed, contains the <see cref="T:System.Net.Http.HttpResponseMessage"/>.
        /// </returns>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken);
    }
}
