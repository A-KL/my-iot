namespace Windows.Http
{
    using global::System;
    using global::System.Net;

    public sealed class ListenerPrefix
    {
        private readonly string original;
        private ushort port;

        public HttpListener Listener;

        public ListenerPrefix(string prefix)
        {
            this.original = prefix;
            this.Parse(prefix);
        }

        public override string ToString()
        {
            return this.original;
        }

        public IPAddress[] Addresses
        {
            get; set;
        }

        public bool Secure
        {
            get; private set;
        }

        public string Host
        {
            get; private set;
        }

        public int Port
        {
            get { return (int)port; }
        }

        public string Path
        {
            get; private set;
        }

        // Equals and GetHashCode are required to detect duplicates in HttpListenerPrefixCollection.
        public override bool Equals(object o)
        {
            var other = o as ListenerPrefix;
            if (other == null)
            {
                return false;
            }

            return this.original == other.original;
        }

        public override int GetHashCode()
        {
            return this.original.GetHashCode();
        }

        private void Parse(string uri)
        {
            var default_port = (uri.StartsWith("http://")) ? 80 : -1;

            if (default_port == -1)
            {
                default_port = (uri.StartsWith("https://")) ? 443 : -1;
                this.Secure = true;
            }

            var length = uri.Length;
            var startHost = uri.IndexOf(':') + 3;
            if (startHost >= length)
            {
                throw new ArgumentException("No host specified.");
            }

            var colon = uri.IndexOf(':', startHost, length - startHost);
            int root;

            if (colon > 0)
            {
                this.Host = uri.Substring(startHost, colon - startHost);
                root = uri.IndexOf('/', colon, length - colon);
                this.port = (ushort)int.Parse(uri.Substring(colon + 1, root - colon - 1));
                this.Path = uri.Substring(root);
            }
            else
            {
                root = uri.IndexOf('/', startHost, length - startHost);
                this.Host = uri.Substring(startHost, root - startHost);
                this.Path = uri.Substring(root);
            }

            if (this.Path.Length != 1)
            {
                this.Path = this.Path.Substring(0, this.Path.Length - 1);
            }
        }

        public static void CheckUri(string uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uriPrefix");
            }

            var defaultPort = uri.StartsWith("http://") ? 80 : -1;

            if (defaultPort == -1)
            {
                defaultPort = uri.StartsWith("https://") ? 443 : -1;
            }
            if (defaultPort == -1)
            {
                throw new ArgumentException("Only 'http' and 'https' schemes are supported.");
            }

            var length = uri.Length;
            var startHost = uri.IndexOf(':') + 3;

            if (startHost >= length)
            {
                throw new ArgumentException("No host specified.");
            }

            var colon = uri.IndexOf(':', startHost, length - startHost);
            if (startHost == colon)
            {
                throw new ArgumentException("No host specified.");
            }

            int root;
            if (colon > 0)
            {
                root = uri.IndexOf('/', colon, length - colon);
                if (root == -1)
                {
                    throw new ArgumentException("No path specified.");
                }

                try
                {
                    var p = int.Parse(uri.Substring(colon + 1, root - colon - 1));
                    if (p <= 0 || p >= 65536)
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    throw new ArgumentException("Invalid port.");
                }
            }
            else
            {
                root = uri.IndexOf('/', startHost, length - startHost);
                if (root == -1)
                {
                    throw new ArgumentException("No path specified.");
                }
            }

            if (uri[uri.Length - 1] != '/')
            {
                throw new ArgumentException("The prefix must end with '/'");
            }
        }
    }
}