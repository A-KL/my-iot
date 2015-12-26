namespace Microsoft.Iot.Extended.Sensors
{
    public class SensorData
    {
        public uint AccelerationX { get; private set; }

        public uint AccelerationY { get; private set; }

        public uint AccelerationZ { get; private set; }

        public uint Temperature { get; private set; }

        public uint GyroscopeX { get; private set; }

        public uint GyroscopeY { get; private set; }

        public uint GyroscopeZ { get; private set; }

        public SensorData(byte[] buffer)
        {
            // Ergebnis für den Beschleunigungssensors zusammenlegen durch Bitshifting
            this.AccelerationX = (((uint)buffer[0]) << 8) | buffer[1];
            this.AccelerationY = (((uint)buffer[2]) << 8) | buffer[3];
            this.AccelerationZ = (((uint)buffer[4]) << 8) | buffer[5];

            // Ergebnis für Temperatur
            this.Temperature = (((uint)buffer[6]) << 8) | buffer[7];

            // Ergebnis für den Gyroskopsensors zusammenlegen durch Bitshifting
            this.GyroscopeX = (((uint)buffer[8]) << 8) | buffer[9];
            this.GyroscopeY = (((uint)buffer[10]) << 8) | buffer[11];
            this.GyroscopeZ = (((uint)buffer[12]) << 8) | buffer[13];
        }
    }
}
