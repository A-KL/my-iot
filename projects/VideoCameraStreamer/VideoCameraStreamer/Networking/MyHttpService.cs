namespace VideoCameraStreamer.Networking
{
    using System.IO;
    using System.Net;
    using System.Text;
    using Griffin.Networking.Buffers;
    using Griffin.Networking.Protocol.Http;
    using Griffin.Networking.Protocol.Http.Protocol;

    public class MyHttpService : HttpService
    {
        private static readonly BufferSliceStack Stack = new BufferSliceStack(50, 32000);

        public MyHttpService()
            : base(Stack)
        {
        }

        public override void Dispose()
        {
        }

        public override void OnRequest(IRequest request)
        {
            var response = request.CreateResponse(HttpStatusCode.OK, "Welcome");

            response.Body = new MemoryStream();
            response.ContentType = "text/plain";
            
            var buffer = Encoding.UTF8.GetBytes("Hello world");

            response.Body.Write(buffer, 0, buffer.Length);
            response.Body.Position = 0;

            this.Send(response);
        }
    }
}