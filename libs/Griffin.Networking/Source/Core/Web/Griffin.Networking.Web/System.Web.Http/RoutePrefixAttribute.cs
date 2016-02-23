namespace System.Web.Http
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RoutePrefixAttribute : Attribute
    {
        public string Template { get; set; }

        public RoutePrefixAttribute(string template)
        {
            this.Template = template;
        }
    }
}