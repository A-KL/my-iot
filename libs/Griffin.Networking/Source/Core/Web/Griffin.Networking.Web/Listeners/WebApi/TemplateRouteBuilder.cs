namespace Griffin.Networking.Web.Listeners.WebApi
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class TemplateRouteBuilder
    {
        private readonly Regex paramRegex = new Regex(@"{(\w+)}", RegexOptions.IgnoreCase);

        private readonly IDictionary<string, object> parameters = new Dictionary<string, object>();

        private readonly string template;

        public TemplateRouteBuilder(string template)
        {
            this.template = template;

            foreach (Match match in this.paramRegex.Matches(this.template))
            {
                this.parameters.Add(match.Groups[1].Value, null);
            }
        }

        public IDictionary<string, object> Parameters
        {
            get { return this.parameters; }
        }

        public override string ToString()
        {
            var result = string.Empty;

            foreach (var parametr in this.parameters)
            {
                result = this.template.Replace("{" + parametr.Key + "}", parametr.Value.ToString());
            }

            return result;
        }

        public static bool HasParamets(string template)
        {
            if (string.IsNullOrEmpty(template))
            {
                return false;
            }

            return template.Contains('{') && template.Contains('}');
        }
    }
}