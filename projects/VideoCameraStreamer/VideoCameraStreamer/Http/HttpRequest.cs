using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace VideoCameraStreamer.Http
{
    using System.Collections.Generic;

    enum HttpParserState
    {
        METHOD,
        URL,
        URLPARM,
        URLVALUE,
        VERSION,
        HEADERKEY,
        HEADERVALUE,
        BODY,
        OK
    };

    public enum HttpResponseState
    {
        OK = 200,
        BAD_REQUEST = 400,
        NOT_FOUND = 404
    }

    public class HttpRequest
    {
        public string Method { get; set; }

        public string Url { get; set; }

        public string Version { get; set; }

        public IEnumerable<KeyValuePair<string, string>> UrlParametes { get; set; }

        public bool Execute { get; set; }

        public IEnumerable<KeyValuePair<string, string>> Headers { get; set; }

        public int Size { get; set; }

        public string Content { get; set; }
    }

    public sealed class HttpRequestParser
    {
        private HttpParserState ParserState;

        private readonly HttpRequest Request;

        private const uint DefaultBufferSize = 8192;

        private readonly uint BufferSize = DefaultBufferSize;

        public HttpRequestParser(uint readBuffer)
        {
            this.Request = new HttpRequest();
            this.BufferSize = readBuffer;
        }

        public HttpRequestParser() :
            this(DefaultBufferSize)
        {

        }

        public IAsyncOperation<HttpRequest> GetHttpRequestForStream(IInputStream stream)
        {
            return this.ProcessStream(stream).AsAsyncOperation();
        }

        private async Task<HttpRequest> ProcessStream(IInputStream stream)
        {
            Dictionary<string, string> _httpHeaders = null;
            Dictionary<string, string> _urlParameters = null;

            var data = new byte[BufferSize];
            var requestString = new StringBuilder();
            var dataRead = this.BufferSize;

            var buffer = data.AsBuffer();

            var hValue = string.Empty;
            var hKey = string.Empty;

            try
            {
                // binary data buffer index
                uint bfndx = 0;

                // Incoming message may be larger than the buffer size.
                while (dataRead == BufferSize)
                {
                    await stream.ReadAsync(buffer, BufferSize, InputStreamOptions.Partial);
                    requestString.Append(Encoding.UTF8.GetString(data, 0, data.Length));
                    dataRead = buffer.Length;

                    // read buffer index
                    uint ndx = 0;
                    do
                    {
                        switch (this.ParserState)
                        {
                            case HttpParserState.METHOD:
                                if (data[ndx] != ' ')
                                {
                                    this.Request.Method += (char)buffer.GetByte(ndx++);
                                }
                                else
                                {
                                    ndx++;
                                    ParserState = HttpParserState.URL;
                                }
                                break;

                            case HttpParserState.URL:
                                if (data[ndx] == '?')
                                {
                                    ndx++;
                                    hKey = string.Empty;
                                    this.Request.Execute = true;
                                    _urlParameters = new Dictionary<string, string>();
                                    this.ParserState = HttpParserState.URLPARM;
                                }
                                else if (data[ndx] != ' ')
                                    this.Request.Url += (char)buffer.GetByte(ndx++);
                                else
                                {
                                    ndx++;
                                    this.Request.Url = WebUtility.UrlDecode(this.Request.Url);
                                    ParserState = HttpParserState.VERSION;
                                }
                                break;

                            case HttpParserState.URLPARM:
                                if (data[ndx] == '=')
                                {
                                    ndx++;
                                    hValue = "";
                                    this.ParserState = HttpParserState.URLVALUE;
                                }
                                else if (data[ndx] == ' ')
                                {
                                    ndx++;

                                    this.Request.Url = WebUtility.UrlDecode(this.Request.Url);
                                    ParserState = HttpParserState.VERSION;
                                }
                                else
                                {
                                    hKey += (char)buffer.GetByte(ndx++);
                                }
                                break;

                            case HttpParserState.URLVALUE:
                                if (data[ndx] == '&')
                                {
                                    ndx++;
                                    hKey = WebUtility.UrlDecode(hKey);
                                    hValue = WebUtility.UrlDecode(hValue);
                                    _urlParameters[hKey] = _urlParameters.ContainsKey(hKey) ? _urlParameters[hKey] + ", " + hValue : hValue;
                                    hKey = "";
                                    ParserState = HttpParserState.URLPARM;
                                }
                                else if (data[ndx] == ' ')
                                {
                                    ndx++;
                                    hKey = WebUtility.UrlDecode(hKey);
                                    hValue = WebUtility.UrlDecode(hValue);
                                    _urlParameters[hKey] = _urlParameters.ContainsKey(hKey) ? _urlParameters[hKey] + ", " + hValue : hValue;
                                    this.Request.Url = WebUtility.UrlDecode(this.Request.Url);
                                    ParserState = HttpParserState.VERSION;
                                }
                                else
                                {
                                    hValue += (char)buffer.GetByte(ndx++);
                                }
                                break;

                            case HttpParserState.VERSION:
                                if (data[ndx] == '\r')
                                {
                                    ndx++;
                                }
                                else if (data[ndx] != '\n')
                                {
                                    this.Request.Version += (char)buffer.GetByte(ndx++);
                                }
                                else
                                {
                                    ndx++;
                                    hKey = "";
                                    _httpHeaders = new Dictionary<string, string>();
                                    this.ParserState = HttpParserState.HEADERKEY;
                                }
                                break;

                            case HttpParserState.HEADERKEY:
                                if (data[ndx] == '\r')
                                {
                                    ndx++;
                                }
                                else if (data[ndx] == '\n')
                                {
                                    ndx++;

                                    if (_httpHeaders.ContainsKey("Content-Length"))
                                    {
                                        this.Request.Size = Convert.ToInt32(_httpHeaders["Content-Length"]);
                                        ParserState = HttpParserState.BODY;
                                    }
                                    else
                                    {
                                        ParserState = HttpParserState.OK;
                                    }
                                }
                                else if (data[ndx] == ':')
                                {
                                    ndx++;
                                }
                                else if (data[ndx] != ' ')
                                    hKey += (char)buffer.GetByte(ndx++);
                                else
                                {
                                    ndx++;
                                    hValue = "";
                                    this.ParserState = HttpParserState.HEADERVALUE;
                                }
                                break;

                            case HttpParserState.HEADERVALUE:
                                if (data[ndx] == '\r')
                                {
                                    ndx++;
                                }
                                else if (data[ndx] != '\n')
                                {
                                    hValue += (char)buffer.GetByte(ndx++);
                                }
                                else
                                {
                                    ndx++;
                                    _httpHeaders.Add(hKey, hValue);
                                    hKey = "";
                                    ParserState = HttpParserState.HEADERKEY;
                                }
                                break;

                            case HttpParserState.BODY:
                                // Append to request BodyData
                                this.Request.Content = Encoding.UTF8.GetString(data, 0, this.Request.Size);
                                bfndx += dataRead - ndx;
                                ndx = dataRead;
                                if (this.Request.Size <= bfndx)
                                {
                                    this.ParserState = HttpParserState.OK;
                                }
                                break;
                                //default:
                                //   ndx++;
                                //   break;

                        }
                    }
                    while (ndx < dataRead);
                };

                // Print out the received message to the console.
                Debug.WriteLine("You received the following message : \n" + requestString);
                if (_httpHeaders != null)
                {
                    this.Request.Headers = _httpHeaders.AsEnumerable();
                }

                if (_urlParameters != null)
                {
                    this.Request.UrlParametes = _urlParameters.AsEnumerable();
                }

                return this.Request;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            return null;
        }

    }
}
