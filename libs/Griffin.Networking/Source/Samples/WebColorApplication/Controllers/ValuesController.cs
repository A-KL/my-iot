using System.Collections.Generic;
using System.Web.Http;
using WebColorApplication.Model;

namespace WebColorApplication.Controllers
{
    [RoutePrefix("api/{controller}")]
    public class ValuesController : ApiController
    {
        public ValuesController(IAdcService service)
        {

        }

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
