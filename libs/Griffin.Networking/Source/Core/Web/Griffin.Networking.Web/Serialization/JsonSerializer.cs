namespace Griffin.Networking.Web.Serialization
{
    using System;
    using System.IO;
    using Newtonsoft.Json;

    public class HttpJsonSerializer : IHttpSerializer
    {
        private readonly JsonSerializer serializer = new JsonSerializer();

        public string ContentType
        {
            get { return HttpContentType.Json; }
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public string Serialize(object data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public void Serialize(object data, Stream stream)
        {
            using (var writer = new StreamWriter(stream))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                serializer.Serialize(jsonWriter, data);
            }

        }

        public object Deserialize(Stream stream, Type targetType)
        {
            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return this.serializer.Deserialize(jsonTextReader, targetType);
            }
        }
    }
}
