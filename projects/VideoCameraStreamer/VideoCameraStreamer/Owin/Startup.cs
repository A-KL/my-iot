using Windows.Foundation.Diagnostics;
using Owin;

namespace VideoCameraStreamer.Owin
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
           // app.Use<LoggerMiddleware>(new FileLoggingSession("session"));

            //var config = new HttpConfiguration();
            //// configure Web API 
            //// app.UseWebApi(config);

            //// additional middleware registrations         

            //app.Run(context =>
            //{
            //    context.Response.ContentType = "text/plain";
            //    return context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
