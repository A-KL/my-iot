namespace VideoCameraStreamer.Owin
{
    using System.Threading.Tasks;
    using Windows.Foundation.Diagnostics;

    using Microsoft.Owin;

    public class LoggerMiddleware : OwinMiddleware
    {
        private readonly IFileLoggingSession fileLoggingSession;
        private readonly ILoggingChannel loggingChannel;

        public LoggerMiddleware(OwinMiddleware next, IFileLoggingSession logger) : base(next)
        {
            this.fileLoggingSession = logger;

            this.loggingChannel = new LoggingChannel("channel");

            fileLoggingSession.AddLoggingChannel(loggingChannel);
        }

        public override async Task Invoke(IOwinContext context)
        {
            loggingChannel.LogMessage("Middleware begin", LoggingLevel.Information);

            await this.Next.Invoke(context);

            loggingChannel.LogMessage("Middleware end", LoggingLevel.Information);
        }
    }
}
