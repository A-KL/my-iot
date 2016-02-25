using Griffin.Networking.Protocol.Http.Protocol;

namespace Griffin.Networking.Web.Serialization
{
    public interface ISerializationFactory
    {
        IHttpSerializer Create(IRequest request);
    }
}