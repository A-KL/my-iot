using System;
using System.Collections.Generic;

namespace WebServerDemo.Model
{
    public interface IWeatherService
    {
        IEnumerable<TemperatureInfo> GetTemperatureHistory(DateTime start, TimeSpan duration);

        TemperatureInfo GetCurrentTemperature(int sensorId);

        void ClearSensorData(int sensorId);
    }
}
