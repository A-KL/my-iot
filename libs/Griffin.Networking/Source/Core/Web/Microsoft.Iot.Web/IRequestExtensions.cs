using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Griffin.Networking.Protocol.Http.Protocol;

namespace Microsoft.Iot.Web
{
    public static class IRequestExtensions
    {
        public static HttpRequestMessage ToHttpRequestMessage(this IRequest message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
                //throw Error.ArgumentNull("message");
            }

            var request = new HttpRequestMessage(new HttpMethod(message.Method), message.Uri);

            foreach (var header in message.Headers)
            {
                request.Headers.Add(header.Name, header.Value);
            }

            if (message.Form.Count > 0)
            {
                request.Content = new FormUrlEncodedContent(message.Form.Select(f => new KeyValuePair<string, string>(f.Name, f.Value)));
            }

            if (message.Files.Count > 0)
            {
                //request.Content = new MultipartFormDataContent();
                throw new ArgumentException("Files are not implemented");               
            }

            return request;
        }
    }
}
