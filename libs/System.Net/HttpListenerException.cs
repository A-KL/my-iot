namespace System.Net
{
    using System.ComponentModel;

    public class HttpListenerException : Win32Exception
    {
        public HttpListenerException()
        {
        }

        public HttpListenerException(int errorCode) : base(errorCode)
        {
        }

        public HttpListenerException(int errorCode, string message) : base(errorCode, message)
        {
        }
    }
}

