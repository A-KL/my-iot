namespace Windows.Http.Extensions
{
    using global::System.Net;
    using Networking.Sockets;

    public static class StreamSocketInformationExtensions
    {
        public static IPEndPoint LocalEndPoint(this StreamSocketInformation information)
        {
            var address = IPAddress.Parse(information.LocalAddress.RawName);

            return new IPEndPoint(address, int.Parse(information.LocalPort));
        }

        public static IPEndPoint RemoteEndPoint(this StreamSocketInformation information)
        {
            var address = IPAddress.Parse(information.RemoteAddress.RawName);

            return new IPEndPoint(address, int.Parse(information.RemotePort));
        }
    }
}
