namespace Microsoft.Iot.Extended.Audio.VLSI
{
    using System;
    using System.Threading.Tasks;
    using Windows.Devices.Enumeration;
    using Windows.Devices.Gpio;
    using Windows.Devices.Spi;

    public class AudioDevice
    {
        private SpiDevice hardware;
        private GpioController gpio;

        protected async Task InitSpi()
        {
            var settings = new SpiConnectionSettings(0) // SPI_CHIP_SELECT_LINE
            {
                ClockFrequency = 10000000,
                Mode = SpiMode.Mode3
            }; 

            /* Create SPI initialization settings                               */
            /* Datasheet specifies maximum SPI clock frequency of 10MHz         */
            /* The display expects an idle-high clock polarity, we use Mode3
             * to set the clock polarity and phase to: CPOL = 1, CPHA = 1        */

            var spiAqs = SpiDevice.GetDeviceSelector("spi0");       /* Find the selector string for the SPI bus controller          */
            var devicesInfo = await DeviceInformation.FindAllAsync(spiAqs);         /* Find the SPI bus controller device with our selector string  */
            this.hardware = await SpiDevice.FromIdAsync(devicesInfo[0].Id, settings);  /* Create an SpiDevice with our bus controller and SPI settings */
        }

        protected void InitGpio()
        {
            this.gpio = GpioController.GetDefault(); /* Get the default GPIO controller on the system */

            /* Initialize a pin as output for the Data/Command line on the display  */
            //DataCommandPin = IoController.OpenPin(DATA_COMMAND_PIN);
            //DataCommandPin.Write(GpioPinValue.High);
            //DataCommandPin.SetDriveMode(GpioPinDriveMode.Output);

            ///* Initialize a pin as output for the hardware Reset line on the display */
            //ResetPin = IoController.OpenPin(RESET_PIN);
            //ResetPin.Write(GpioPinValue.High);
            //ResetPin.SetDriveMode(GpioPinDriveMode.Output);
        }

        private readonly byte[] cmdBuffer = new byte[4];

        protected void WriteRegister(byte register, ushort data)
        {
            while (!dreq.Read())
            { }

            //this.hardware.ConnectionSettings.

            this.cmdBuffer[0] = (byte)Vs1053_SPI.CMD_WRITE;
            this.cmdBuffer[1] = register;
            this.cmdBuffer[2] = (byte)(data >> 8);
            this.cmdBuffer[3] = (byte)data;

            this.hardware.Write(this.cmdBuffer);
        }

        /// <summary>
        /// Reads 16bit value from a register
        /// </summary>
        /// <param name="register">Source register</param>
        /// <returns>16bit value from the source register</returns>
        protected ushort CommandRead(byte register)
        {
            ushort temp;

            while (dreq.Read() == false)
                Thread.Sleep(1);

            // spi.Config = cmdConfig;
            cmdBuffer[0] = CMD_READ;

            cmdBuffer[1] = register;
            cmdBuffer[2] = 0;
            cmdBuffer[3] = 0;

            SpiManager.LockWriteRead(cmdConfig, cmdBuffer, cmdBuffer, 2);

            temp = cmdBuffer[0];
            temp <<= 8;

            temp += cmdBuffer[1];

            return temp;
        }
    }
}
