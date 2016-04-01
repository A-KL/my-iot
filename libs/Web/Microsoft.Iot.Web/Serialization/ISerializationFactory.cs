using Griffin.Net.Protocols.Http;

namespace Microsoft.Iot.Web.Serialization
{
    public interface ISerializationFactory
    {
        IHttpSerializer Create(IHttpRequest request);
    }
}