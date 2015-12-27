namespace Microsoft.Iot.Extended.Sensors
{
    using System;
    using System.Threading.Tasks;
    using Windows.Devices.Enumeration;

    using Windows.Devices.I2c;

    public class MPU6050 : IDisposable
    {
        private const int Address = 0x68;

        private I2cDevice device;
        
        public async Task Init()
        {
            var settings = new I2cConnectionSettings(Address)
            {
                BusSpeed = I2cBusSpeed.StandardMode /* 100KHz bus speed */
            };

            var aqs = I2cDevice.GetDeviceSelector();

            var dis = await DeviceInformation.FindAllAsync(aqs);

            this.device = await I2cDevice.FromIdAsync(dis[0].Id, settings);

            // Sensor Initialisieren
            // Sleep und Reset durchführen
            // Der Hex Wert 0x6B steht für das Power Management 1
            // Der zweite Hex Wert beinhaltet das 'Bit' für einen Reset.
            this.Write(0x6B, 0x80);

            // Sleep beenden und Clock einstelllen
            // Der Hex Wert 0x6B steht für das Power Management 1
            // Der zweite Hex Wert 0x00 legt die 'Clock Select'
            // auf 'Internal 8MHz oscillator'
            // Falls der Temperatur Sensor nicht ausgelesen werden soll,
            // dann kann stattdessen der Wert 0x08 eingetragen werden.
            this.Write(0x6B, 0x00);

            // Konfiguration festlegen
            // Diese Einstellungen aktiviert den Tief Pass Filter (DLPF) und
            // wird z.B. verwendet, andere Vibrationen heraus zu filtern.
            // Setting => Acc=5Hz, Delay=19.0ms, Gyro=5Hz, Delay=18.6ms, Fs=1kHz
            this.Write(0x1A, 0x06);
        }

        public SensorData ReadData()
        {
            var buffer = new byte[14];
            buffer[0] = 0x3B;

            // Der Hex Wert stellt das erste Byte da
            // für die Beschleunigungsachse X
           // this.Write(0x3B);

            // Byte Array übergeben zum beschreiben
            //this.(buffer);
            this.device.WriteRead(new byte[] { 0x3B }, buffer);

            // Ergebnis an das Sensor Objekt, 
            // in dem die Byte Werte umgewandelt werden
            return new SensorData(buffer);        
        }

        public void Dispose()
        {
            this.device.Dispose();
        }
        

        private void Write(params byte[] data)
        {
            this.device.Write(data);
        }
    }
}
