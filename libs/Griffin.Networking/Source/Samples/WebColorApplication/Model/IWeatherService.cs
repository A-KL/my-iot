using System;
using System.Collections.Generic;

namespace WebColorApplication.Model
{
    public interface IWeatherService
    {
        IEnumerable<TemperatureInfo> GetTemperatureHistory(DateTime start, TimeSpan duration);

        TemperatureInfo GetCurrentTemperature(int sensorId);

        void ClearSensorData(int sensorId);
    }
}
