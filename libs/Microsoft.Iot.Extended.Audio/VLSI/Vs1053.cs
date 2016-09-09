// ReSharper disable ConsiderUsingConfigureAwait
namespace Microsoft.Iot.Extended.Audio.VLSI
{
    using System;
    using System.Threading.Tasks;

    public class Vs1053 : SpiAudioDevice
    {
        //private readonly OutputPort redLedPort = new OutputPort(RedLedPin, false);
        // private readonly OutputPort greenLedPort = new OutputPort(GreenLedPin, false);

        private readonly byte[] sampleBuffer = new byte[] { (byte)Vs1053_SPI.CMD_READ, (byte)Vs1053_REGISTERS.SCI_HDAT0 };

        private Vs1053()
        { }

        public static async Task<Vs1053> CreateAsync()
        {
            var device = new Vs1053();

            device.InitGpio();

            await device.InitSpiAsync();

            return device;
        }

        public Task InitRecordingAsync()
        {
            ushort volume = 0x0103;

            this.CommandWrite((byte)Vs1053_REGISTERS.SCI_AICTRL0, 16000);
            this.CommandWrite((byte)Vs1053_REGISTERS.SCI_AICTRL1, 0);
            this.CommandWrite((byte)Vs1053_REGISTERS.SCI_AICTRL2, 4096);
            this.CommandWrite((byte)Vs1053_REGISTERS.SCI_AICTRL3, 2); // 2 or 3 0x40

            this.CommandWrite((byte)Vs1053_REGISTERS.SCI_CLOCKF, 0xa000);
            this.CommandWrite((byte)Vs1053_REGISTERS.SCI_VOL, volume);  // highest volume -1


            // this.CommandWrite((byte)Vs1053_REGISTERS.SCI_MODE, (ushort)Vs_SCI_MODE.SM_SDINEW);
            this.CommandWrite((byte)Vs1053_REGISTERS.SCI_MODE, (ushort)(Vs_SCI_MODE.SM_RESET | Vs_SCI_MODE.SM_ADPCM));

            this.WritePatchFromArray(VsPatches.IMA_ADPCM_Encoder_Fix);

            var read = this.CommandRead((byte)Vs1053_REGISTERS.SCI_VOL);

            if (read != volume)
            {
                throw new Exception("Failed to initialize audio");
            }

            return Task.FromResult(true);
        }

        public void Read()
        {
            var samplesAvailable = this.CommandRead((byte)Vs1053_REGISTERS.SCI_HDAT1); // ? samples count

            byte[] data = new byte[2];

            for (int i = 0; i < samplesAvailable; i++)
            {
                this.CommandRead(sampleBuffer, data);
            }

            var samplesCount = this.CommandRead((byte)Vs1053_REGISTERS.SCI_HDAT0);
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
    }
}
