namespace Griffin.Networking.Web.Serialization
{
    using System;
    using System.IO;
    using Newtonsoft.Json;

    public class HttpJsonSerializer : IHttpSerializer
    {
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public string Serialize(object data)
        {
           return JsonConvert.SerializeObject(data);
        }

        public object Deserialize(Stream stream, Type targetType)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize(jsonTextReader, targetType);
            }
        }
    }
}
