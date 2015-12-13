using System;
using System.Net;
using System.Security.Principal;

namespace Windows.Http
{
    public sealed class HttpListenerContext
    {
        internal HttpListener Listener;

        internal HttpListenerContext(HttpConnection connection)
        {
            this.Connection = connection;

            this.Request = new HttpListenerRequest(this);
            this.Response = new HttpListenerResponse(this);
        }

        internal int ErrorStatus { get; set; } = 400;

        internal string ErrorMessage { get; set; }

        internal bool HaveError
        {
            get { return (ErrorMessage != null); }
        }
        
        public HttpListenerRequest Request { get; }

        public HttpListenerResponse Response { get; }

        public IPrincipal User { get; private set; }


        internal HttpConnection Connection { get; }

        internal void ParseAuthentication(AuthenticationSchemes expectedSchemes)
        {
            if (expectedSchemes == AuthenticationSchemes.Anonymous)
                return;

            // TODO: Handle NTLM/Digest modes
            var header = Request.Headers["Authorization"];
            if (header == null || header.Length < 2)
            {
                return;
            }

            var authenticationData = header.Split(new [] { ' ' }, 2);

            if (string.Compare(authenticationData[0], "basic", StringComparison.OrdinalIgnoreCase) == 0)
            {
                User = this.ParseBasicAuthentication(authenticationData[1]);
            }
            // TODO: throw if malformed -> 400 bad request
        }

        internal IPrincipal ParseBasicAuthentication(string authData)
        {
            try
            {
                // Basic AUTH Data is a formatted Base64 String
                //string domain = null;
                string user;
                string password;
                int pos;

                var authString = global::System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(authData));

                // The format is DOMAIN\username:password
                // Domain is optional

                pos = authString.IndexOf(':');

                // parse the password off the end
                password = authString.Substring(pos + 1);

                // discard the password
                authString = authString.Substring(0, pos);

                // check if there is a domain
                pos = authString.IndexOf('\\');

                if (pos > 0)
                {
                    //domain = authString.Substring (0, pos);
                    user = authString.Substring(pos);
                }
                else
                {
                    user = authString;
                }

                var identity = new HttpListenerBasicIdentity(user, password);

                // TODO: What are the roles MS sets
                return new GenericPrincipal(identity, new string[0]);
            }
            catch (Exception)
            {
                // Invalid auth data is swallowed silently
                return null;
            }
        }
    }
}