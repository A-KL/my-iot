using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Griffin.Networking.Web.System.Web.Http;
using WebServerDemo.Model;

namespace WebServerDemo.Controllers
{
    [RoutePrefix("api/{controller}")]
    public class TemperatureController : ApiController
    {
        private readonly IWeatherService service;

        public TemperatureController(IWeatherService service)
        {
            this.service = service;
        }

        [HttpGet, Route("")]
        public async Task<IEnumerable<TemperatureInfo>> GetFor10Days()
        {
            var period = TimeSpan.FromDays(10);

            return this.service.GetTemperatureHistory(DateTime.Now - period, period);
        }

        [HttpGet, Route("bysensor/{sensorId}")]
        public TemperatureInfo GetBySensorId(int sensorId)
        {
            return this.service.GetCurrentTemperature(sensorId);
        }

        [HttpDelete, Route("bysensor/{sensorId}")]
        public void Clear(int sensorId)
        {
            this.service.ClearSensorData(sensorId);
        }

        [HttpPost, Route("bysensor/{sensorId}")]
        public void New(int sensorId, TemperatureInfo info)
        {
            
        }
    }
}
