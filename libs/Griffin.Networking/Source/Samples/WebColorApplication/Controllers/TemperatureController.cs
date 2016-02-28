using System;
using System.Collections.Generic;
using System.Web.Http;
using Griffin.Networking.Web.System.Web.Http;
using WebColorApplication.Model;

namespace WebColorApplication.Controllers
{
    [RoutePrefix("api/{controller}")]
    public class TemperatureController : ApiController
    {
        private readonly IWeatherService service;

        public TemperatureController(IWeatherService service)
        {
            this.service = service;
        }

        [HttpGet("")]
        public IEnumerable<TemperatureInfo> GetFor10Days()
        {
            var period = TimeSpan.FromDays(10);

            return this.service.GetTemperatureHistory(DateTime.Now - period, period);
        }

        [HttpGet("bysensor/{sensorId}")]
        public TemperatureInfo GetBySensorId(int sensorId)
        {
            return this.service.GetCurrentTemperature(sensorId);
        }

        [HttpDelete("bysensor/{sensorId}")]
        public void Save(int sensorId)
        {
            this.service.ClearSensorData(sensorId);
        }
    }
}
