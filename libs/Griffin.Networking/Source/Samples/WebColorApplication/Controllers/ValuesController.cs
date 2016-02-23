using System.Collections.Generic;
using System.Web.Http;

namespace WebColorApplication.Controller
{
    [RoutePrefix("api/{controller}")]
    public class ValuesController : ApiController
    {
        [HttpGet("")]
        public IEnumerable<string> Get()
        {
            return new[] { "test", "value" };
        }
    }
}
