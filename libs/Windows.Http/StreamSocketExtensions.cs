namespace Windows.Http
{
    using global::System;
    using global::System.Runtime.InteropServices.WindowsRuntime;
    using global::System.Text;
    using global::System.Threading.Tasks;

    using Storage.Streams;


    public static class StreamSocketExtensions
    {
        public static async Task<string> ReadLine(this IInputStream inputStream)
        {
            var buffer = new byte[1];

            var result = string.Empty;

            while (true)
            {
                await inputStream.ReadAsync(buffer.AsBuffer(), (uint)buffer.Length, InputStreamOptions.Partial);

                result += Encoding.ASCII.GetString(buffer);

                if (result.EndsWith(Environment.NewLine))
                {
                    return result;
                }
            }
        }
    }
    

    //internal async static Task<HttpRequest> Parse(StreamSocket socket)
    //{
    //    var flag = true; // just so we know we are still reading

    //    var headerString = string.Empty; // to store header information

    //    var contentLength = 0; // the body length

    //    var buffer = new byte[1]; // read the header byte by byte, until \r\n\r\n

    //    using (var input = socket.InputStream)
    //    {
    //        while (true)
    //        {
    //            await inputStream.ReadAsync(buffer.AsBuffer(), (uint)buffer.Length, InputStreamOptions.Partial);

    //            headerString += Encoding.ASCII.GetString(buffer);

    //            if (headerString.Contains("\r\n\r\n"))
    //            {
    //                // header is received, parsing content length
    //                // I use regular expressions, but any other method you can think of is ok
    //                var reg = new Regex("\\\r\nContent-Length: (.*?)\\\r\n");
    //                var m = reg.Match(headerString);
    //                // contentLength = int.Parse(m.Groups[1].ToString());
    //                break;
    //            }
    //        }
    //    }

    //    return new HttpRequest(null);
    //}
}
