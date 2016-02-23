namespace System.Web.Http
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RouteAttribute : Attribute
    {
        public string Template { get; set; }

        public RouteAttribute(string template)
        {
            this.Template = template;
        }
    }
}