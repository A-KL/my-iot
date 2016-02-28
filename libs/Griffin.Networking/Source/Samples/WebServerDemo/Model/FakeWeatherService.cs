using System;
using System.Collections.Generic;
using System.Linq;

namespace WebServerDemo.Model
{
    public class FakeWeatherService : IWeatherService
    {
        private readonly IList<TemperatureInfo> infos = new List<TemperatureInfo>()
        {
            new TemperatureInfo
            {
                Date = new DateTime(2016, 02, 28, 10, 10, 10),
                SensorId = 0,
                Temperature = 15.5f
            },
            new TemperatureInfo
            {
                Date = new DateTime(2016, 02, 28, 10, 10, 10),
                SensorId = 0,
                Temperature = 15.234f
            },
            new TemperatureInfo
            {
                Date = new DateTime(2015, 02, 28, 10, 10, 10),
                SensorId = 1,
                Temperature = 35.5f
            },
            new TemperatureInfo
            {
                Date = new DateTime(2015, 02, 28, 10, 10, 10),
                SensorId = 2,
                Temperature = 25.5f
            }
        };

        private readonly Random rnd = new Random();

        public IEnumerable<TemperatureInfo> GetTemperatureHistory(DateTime start, TimeSpan duration)
        {
            return from info in this.infos
                where info.Date < start && info.Date <= start + duration
                select info;
        }

        public TemperatureInfo GetCurrentTemperature(int sensorId)
        {
            var newTemperature = new TemperatureInfo { Date = DateTime.Now, SensorId = 0, Temperature = 50 * (float)this.rnd.NextDouble() };

            this.infos.Add(newTemperature);

            return newTemperature;
        }

        public void ClearSensorData(int sensorId)
        {
            var itemsToRemove = (from info in this.infos
                                where info.SensorId == sensorId
                                select info).ToList();

            foreach (var temperatureInfo in itemsToRemove)
            {
                this.infos.Remove(temperatureInfo);
            }

            itemsToRemove.Clear();          
        }
    }
}
