using System.Net.Http;
using System.Web.Http;

namespace WebServerDemo.Controllers
{
    public class CameraController : ApiController
    {
        public HttpResponseMessage GetStream()
        {
           // var imageStream = new ImageStream();
          //  Func<Stream, HttpContent, TransportContext, Task> func = imageStream.WriteToStream;

           // var response = Request.CreateResponse();
            
            //response.Content = new PushStreamContent(func);
            //response.Content.Headers.Remove("Content-Type");
            //response.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/x-mixed-replace;boundary=" + imageStream.Boundary);
            //return response;

            return null;            
        }
    }
}
