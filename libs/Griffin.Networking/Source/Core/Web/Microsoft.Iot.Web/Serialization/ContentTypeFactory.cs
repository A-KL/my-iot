using System;
using Griffin.Networking.Protocol.Http.Protocol;
using Griffin.Networking.Web;

namespace Microsoft.Iot.Web.Serialization
{
    public class ContentTypeFactory : ISerializationFactory
    {
        public IHttpSerializer Create(IRequest request)
        {
            switch (request.ContentType)
            {
                case HttpContentType.Json:
                case null:
                    return new HttpJsonSerializer();
                default:
                    throw new Exception("Content type " + request.ContentType + " is not supported");
            }
        }
    }
}
