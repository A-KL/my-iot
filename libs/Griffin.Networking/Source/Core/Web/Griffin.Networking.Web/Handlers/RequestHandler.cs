namespace Griffin.Networking.Web
{
    using Protocol.Http.Protocol;

    public abstract class RequestHandler
    {
        protected IResponse Ok()
        {
            return null;
        }
    }
}
