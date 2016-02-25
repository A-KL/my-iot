using System.Collections.Generic;
using System.Web.Http;

namespace WebColorApplication.Controllers
{
    [RoutePrefix("api/{controller}")]
    public class ValuesController : ApiController
    {
        [HttpGet("")]
        public IEnumerable<string> GetAll()
        {
            return new[] { "test", "value" };
        }

        [HttpGet("{id}")]
        public string GetById(int id)
        {
            return "test";
        }

        [HttpPost("")]
        public void Save(string[] data)
        {

        }
    }
}
