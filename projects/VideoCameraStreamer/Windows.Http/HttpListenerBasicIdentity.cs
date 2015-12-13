namespace Windows.Http
{
    using global::System.Security.Principal;

    public class HttpListenerBasicIdentity : GenericIdentity
    {
        public HttpListenerBasicIdentity(string username, string password) : base(username, "Basic")
        {
            this.Password = password;
        }

        public virtual string Password { get; }
    }
}

