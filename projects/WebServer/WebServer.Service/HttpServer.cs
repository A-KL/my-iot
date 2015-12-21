namespace WebServer.Service
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using System.IO;
    using System.Runtime.InteropServices.WindowsRuntime;
    using Windows.ApplicationModel.AppService;
    using Windows.Foundation.Collections;
    using Windows.Networking.Sockets;
    using Windows.Storage.Streams;

    public sealed class HttpServer : IDisposable
    {
        private const uint BufferSize = 8192;

        private readonly StreamSocketListener listener;

        private string offHtmlString = "<html><head><title>Blinky App</title></head><body><form action=\"blinky.html\" method=\"GET\"><input type=\"radio\" name=\"state\" value=\"on\" onclick=\"this.form.submit()\"> On<br><input type=\"radio\" name=\"state\" value=\"off\" checked onclick=\"this.form.submit()\"> Off</form></body></html>";
        private string onHtmlString = "<html><head><title>Blinky App</title></head><body><form action=\"blinky.html\" method=\"GET\"><input type=\"radio\" name=\"state\" value=\"on\" checked onclick=\"this.form.submit()\"> On<br><input type=\"radio\" name=\"state\" value=\"off\" onclick=\"this.form.submit()\"> Off</form></body></html>";
        
        private int port = 8000;        
        private AppServiceConnection appServiceConnection;

        public HttpServer(int serverPort, AppServiceConnection connection)
        {
            this.listener = new StreamSocketListener();
            this.port = serverPort;
            this.appServiceConnection = connection;
            this.listener.ConnectionReceived += (s, e) => this.ProcessRequestAsync(e.Socket);
        }

        public void StartServer()
        {
#pragma warning disable CS4014
            this.listener.BindServiceNameAsync(this.port.ToString());
#pragma warning restore CS4014
        }

        public void Dispose()
        {
            this.listener.Dispose();
        }

        private async void ProcessRequestAsync(StreamSocket socket)
        {
            // this works for text only
            var request = new StringBuilder();

            using (var input = socket.InputStream)
            {
                var data = new byte[BufferSize];
                var buffer = data.AsBuffer();
                var dataRead = BufferSize;

                while (dataRead == BufferSize)
                {
                    await input.ReadAsync(buffer, BufferSize, InputStreamOptions.Partial);

                    request.Append(Encoding.UTF8.GetString(data, 0, data.Length));

                    dataRead = buffer.Length;
                }
            }

            using (var output = socket.OutputStream)
            {
                var requestMethod = request.ToString().Split('\n')[0];
                var requestParts = requestMethod.Split(' ');

                if (requestParts[0] == "GET")
                {
                    await this.WriteResponseAsync(requestParts[1], output);
                }
                else
                {
                    throw new InvalidDataException("HTTP method not supported: " + requestParts[0]);
                }
            }
        }

        private async Task WriteResponseAsync(string request, IOutputStream os)
        {
            // See if the request is for blinky.html, if yes get the new state
            var state = "Unspecified";
            var stateChanged = false;

            if (request.Contains("blinky.html?state=on"))
            {
                state = "On";
                stateChanged = true;
            }
            else if (request.Contains("blinky.html?state=off"))
            {
                state = "Off";
                stateChanged = true;
            }

            if (stateChanged)
            {
                var updateMessage = new ValueSet { { "State", state } };
                var responseStatus = await appServiceConnection.SendMessageAsync(updateMessage);
            }

            string html = state == "On" ? onHtmlString : offHtmlString;
            // Show the html 
            using (var resp = os.AsStreamForWrite())
            {
                // Look in the Data subdirectory of the app package
                var bodyArray = Encoding.UTF8.GetBytes(html);
                MemoryStream stream = new MemoryStream(bodyArray);
                string header = String.Format("HTTP/1.1 200 OK\r\n" +
                                  "Content-Length: {0}\r\n" +
                                  "Connection: close\r\n\r\n",
                                  stream.Length);
                byte[] headerArray = Encoding.UTF8.GetBytes(header);
                await resp.WriteAsync(headerArray, 0, headerArray.Length);
                await stream.CopyToAsync(resp);
                await resp.FlushAsync();
            }

        }
    }
}
