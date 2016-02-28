using Griffin.Networking.Protocol.Http.Protocol;

namespace Microsoft.Iot.Web.Serialization
{
    public interface ISerializationFactory
    {
        IHttpSerializer Create(IRequest request);
    }
}