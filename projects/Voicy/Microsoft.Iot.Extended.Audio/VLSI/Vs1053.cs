// ReSharper disable ConsiderUsingConfigureAwait
namespace Microsoft.Iot.Extended.Audio.VLSI
{
    using System;
    using System.Threading.Tasks;

    public class Vs1053 : SpiAudioDevice
    {
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

            return Task.FromResult(true);
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
