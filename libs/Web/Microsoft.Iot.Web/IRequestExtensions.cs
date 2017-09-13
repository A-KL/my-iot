using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Griffin.Net.Protocols.Http;

namespace Microsoft.Iot.Web
{
    public static class IRequestExtensions
    {
        public static HttpRequestMessage ToHttpRequestMessage(this IHttpRequest message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
                //throw Error.ArgumentNull("message");
            }

            var request = new HttpRequestMessage(new HttpMethod(message.HttpMethod), message.Uri);

            foreach (var header in message.Headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
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
