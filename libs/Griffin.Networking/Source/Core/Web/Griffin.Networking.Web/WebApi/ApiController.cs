namespace Griffin.Networking.Web.WebApi
{
    using System;

    public abstract class ApiController : RequestHandler, IDisposable
    {
        protected ApiController()
        {
            
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
