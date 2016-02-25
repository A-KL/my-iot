namespace Griffin.Networking.Web.Serialization
{
    using System;
    using Protocol.Http.Protocol;

    public class ContentTypeFactory : ISerializationFactory
    {
        public IHttpSerializer Create(IRequest request)
        {
            switch (request.ContentType)
            {
                case HttpContentType.Json:
                    return new HttpJsonSerializer();
                default:
                    throw new Exception("Content type " + request.ContentType + " is not supported");
            }
        }
    }
}
