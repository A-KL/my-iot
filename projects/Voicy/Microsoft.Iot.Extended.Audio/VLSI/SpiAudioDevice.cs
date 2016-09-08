// ReSharper disable ConsiderUsingConfigureAwait
namespace Microsoft.Iot.Extended.Audio.VLSI
{
    using System;
    using System.Threading.Tasks;
    using Windows.Devices.Enumeration;
    using Windows.Devices.Gpio;
    using Windows.Devices.Spi;

    public class SpiAudioDevice
    {
        private SpiDevice commands;
        private SpiDevice data;
        private GpioController gpio;
        private GpioPin dataRequestPin;

        private const string SpiModuleName = "SPI0";
        private const int DataRequestPinNumber = 5;

        private const int SpiCmdChipSelectPinNumber = 0;
        private const int SpiDataChipSelectPinNumber = 1;

        private readonly byte[] cmdBuffer = new byte[4];

        protected async Task InitSpiAsync()
        {
            /* Create SPI initialization settings                               */
            /* Datasheet specifies maximum SPI clock frequency of 2MHz         */
            /* The display expects an idle-high clock polarity, we use Mode1
             * to set the clock polarity and phase to: CPOL = 0, CPHA = 1        */

            var spiCmdSettinds = new SpiConnectionSettings(SpiCmdChipSelectPinNumber)
            {
                ClockFrequency = 2000000,
                Mode = SpiMode.Mode1,
                SharingMode = SpiSharingMode.Shared
            };

            var spiDataSettings = new SpiConnectionSettings(SpiDataChipSelectPinNumber)
            {
                ClockFrequency = 2000000,
                Mode = SpiMode.Mode1,
                SharingMode = SpiSharingMode.Shared
            };


            var spiAqs = SpiDevice.GetDeviceSelector(SpiModuleName);
            var devicesInfo = await DeviceInformation.FindAllAsync(spiAqs);
            this.commands = await SpiDevice.FromIdAsync(devicesInfo[0].Id, spiCmdSettinds);
            this.data = await SpiDevice.FromIdAsync(devicesInfo[0].Id, spiDataSettings);
        }
        
        protected void InitGpio()
        {
            this.gpio = GpioController.GetDefault(); /* Get the default GPIO controller on the system */

            this.dataRequestPin = this.gpio.OpenPin(DataRequestPinNumber);

            // Check if input pull-up resistors are supported
            if (this.dataRequestPin.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
            {
                this.dataRequestPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            }
            else
            {
                this.dataRequestPin.SetDriveMode(GpioPinDriveMode.Input);
            }
        }
        
        protected void CommandWrite(byte register, ushort data)
        {
            this.WaitForDataRequest();

            this.cmdBuffer[0] = (byte)Vs1053_SPI.CMD_WRITE;
            this.cmdBuffer[1] = register;
            this.cmdBuffer[2] = (byte)(data >> 8);
            this.cmdBuffer[3] = (byte)data;

            this.commands.Write(this.cmdBuffer);
        }

        /// <summary>
        /// Reads 16bit value from a register
        /// </summary>
        /// <param name="register">Source register</param>
        /// <returns>16bit value from the source register</returns>
        protected ushort CommandRead(byte register)
        {
            ushort temp;

            this.WaitForDataRequest();

            this.cmdBuffer[0] = (byte)Vs1053_SPI.CMD_READ;

            this.cmdBuffer[1] = register;
            this.cmdBuffer[2] = 0;
            this.cmdBuffer[3] = 0;

            this.commands.TransferFullDuplex(this.cmdBuffer, this.cmdBuffer);

            temp = this.cmdBuffer[2];
            temp <<= 8;

            temp += this.cmdBuffer[3];

            return temp;
        }

        protected void WaitForDataRequest()
        {
            while (this.dataRequestPin.Read() == GpioPinValue.Low)
            {
                Task.Delay(1).Wait();
            }
        }

        protected async Task WaitForDataRequestAsync()
        {
            while (this.dataRequestPin.Read() == GpioPinValue.Low)
            {
                await Task.Delay(1);
            }
        }
    }
}
