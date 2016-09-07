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
        private GpioPin dataRequestPin;

        private const string SpiModuleName = "SPI0";
        private const int DataRequestPinNumber = 5;

        private const int SpiCmdChipSelectPinNumber = 0;
        private const int SpiDataChipSelectPinNumber = 1;

        public async Task Init()
        {
            this.InitGpio();

            await this.InitSpi();

            var d = this.CommandRead((byte)Vs1053_REGISTERS.SCI_MODE);

            ushort volume = 0x0103;

            this.CommandWrite((byte)Vs1053_REGISTERS.SCI_AICTRL0, 16000);
            this.CommandWrite((byte)Vs1053_REGISTERS.SCI_AICTRL1, 0);
            this.CommandWrite((byte)Vs1053_REGISTERS.SCI_AICTRL2, 4096);
            this.CommandWrite((byte)Vs1053_REGISTERS.SCI_AICTRL3, 0); // 2 or 3

            this.CommandWrite((byte)Vs1053_REGISTERS.SCI_CLOCKF, 0xa000);
            this.CommandWrite((byte)Vs1053_REGISTERS.SCI_VOL, volume);  // highest volume -1


            // this.CommandWrite((byte)Vs1053_REGISTERS.SCI_MODE, (ushort)Vs_SCI_MODE.SM_SDINEW);
            this.CommandWrite((byte)Vs1053_REGISTERS.SCI_MODE, (ushort)(Vs_SCI_MODE.SM_RESET | Vs_SCI_MODE.SM_ADPCM));
                        
            var read = this.CommandRead((byte)Vs1053_REGISTERS.SCI_VOL);

            if (read != volume)
            {
                throw new Exception("Failed to initialize audio");
            }
        }

        protected async Task InitSpi()
        {
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

            /* Create SPI initialization settings                               */
            /* Datasheet specifies maximum SPI clock frequency of 10MHz         */
            /* The display expects an idle-high clock polarity, we use Mode3
             * to set the clock polarity and phase to: CPOL = 1, CPHA = 1        */

            var spiAqs = SpiDevice.GetDeviceSelector("SPI0");       /* Find the selector string for the SPI bus controller          */
            var devicesInfo = await DeviceInformation.FindAllAsync(spiAqs);         /* Find the SPI bus controller device with our selector string  */
            this.hardware = await SpiDevice.FromIdAsync(devicesInfo[0].Id, spiCmdSettinds);  /* Create an SpiDevice with our bus controller and SPI settings */
        }

        /// <summary>
        /// Performs soft reset
        /// </summary>
        public void Reset()
        {
            this.CommandWrite((byte)Vs1053_REGISTERS.SCI_MODE, (ushort)(Vs_SCI_MODE.SM_SDINEW | Vs_SCI_MODE.SM_RESET));

            this.WaitForDataRequest();

            this.CommandWrite((byte)Vs1053_REGISTERS.SCI_CLOCKF, 0xa000);

            this.WaitForDataRequest();
        }

        protected void InitGpio()
        {
            this.gpio = GpioController.GetDefault(); /* Get the default GPIO controller on the system */

            this.dataRequestPin = this.gpio.OpenPin(DataRequestPinNumber);

            // Check if input pull-up resistors are supported
            if (dataRequestPin.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
            {
                dataRequestPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            }
            else
            {
                dataRequestPin.SetDriveMode(GpioPinDriveMode.Input);
            }

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

        protected void CommandWrite(byte register, ushort data)
        {
            this.WaitForDataRequest();

            this.cmdBuffer[0] = (byte)Vs1053_SPI.CMD_WRITE;
            this.cmdBuffer[1] = register;
            this.cmdBuffer[2] = (byte)(data >> 8);
            this.cmdBuffer[3] = (byte)data;

            this.hardware.Write(this.cmdBuffer);
        }

        private void WaitForDataRequest()
        {
            while (dataRequestPin.Read() == GpioPinValue.Low)
            {
                Task.Delay(1).Wait();
            }
        }

        /// <summary>
        /// Reads 16bit value from a register
        /// </summary>
        /// <param name="register">Source register</param>
        /// <returns>16bit value from the source register</returns>
        protected ushort CommandRead(byte register)
        {
            ushort temp;

            WaitForDataRequest();

            // spi.Config = cmdConfig;
            cmdBuffer[0] = (byte)Vs1053_SPI.CMD_READ;

            cmdBuffer[1] = register;
            cmdBuffer[2] = 0;
            cmdBuffer[3] = 0;

            byte[] readBuffer = new byte[4];

            this.hardware.TransferFullDuplex(this.cmdBuffer, this.cmdBuffer);
            
            temp = cmdBuffer[2];
            temp <<= 8;

            temp += cmdBuffer[3];

            return temp;
        }
    }
}
