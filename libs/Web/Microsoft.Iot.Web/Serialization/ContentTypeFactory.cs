using System;
using System.Web;
using Griffin.Net.Protocols.Http;

namespace Microsoft.Iot.Web.Serialization
{
    public class ContentTypeFactory : ISerializationFactory
    {
        public IHttpSerializer Create(IHttpRequest request)
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
