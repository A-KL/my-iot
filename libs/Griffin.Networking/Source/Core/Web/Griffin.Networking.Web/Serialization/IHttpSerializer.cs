using System;
using System.IO;

namespace Griffin.Networking.Web.Serialization
{
    public interface IHttpSerializer
    {
        string Serialize(object data);

        object Deserialize(Stream stream, Type targetType);
    }
}