namespace System.Web.Http
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpGetAttribute : Attribute
    {
        public HttpGetAttribute(string empty)
        {

        }
    }
}