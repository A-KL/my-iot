using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebServerDemo.Model;

namespace WebServerDemo.Controllers
{
    [RoutePrefix("api/{controller}")]
    public class CameraController : ApiController
    {
        [HttpGet("")]
        public HttpResponseMessage GetStream()
        {
            var imageStream = new MJpegSource();
            Func<Stream, HttpContent, TransportContext, Task> func = imageStream.WriteToStream;

            var response = Request.CreateResponse();
            response.Content = new PushStreamContent(func);
            response.Content.Headers.Remove("Content-Type");
            response.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/x-mixed-replace;boundary=" + imageStream.Boundary);

            return response;
        }
    }
}
