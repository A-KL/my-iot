using System;
using System.Collections.Generic;
using System.Web.Http;

namespace WebColorApplication.Controller
{
    [Route("api/{controller}")]
    public class ValuesController : ApiController
    {
        [HttpGet("")]
        public IEnumerable<string> Get()
        {
            return new[] { "test", "value" };
        }
    }

    public class HttpGetAttribute : Attribute
    {
        public HttpGetAttribute(string empty)
        {

        }
    }

    public class RouteAttribute : Attribute
    {
        public RouteAttribute(string apiController)
        {

        }
    }
}
