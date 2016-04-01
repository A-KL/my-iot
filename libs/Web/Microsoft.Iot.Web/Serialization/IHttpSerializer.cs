using System;
using System.IO;

namespace Microsoft.Iot.Web.Serialization
{
    public interface IHttpSerializer
    {
        string ContentType { get; }

        string Serialize(object data);

        void Serialize(object data, Stream stream);

        object Deserialize(Stream stream, Type targetType);
    }
}