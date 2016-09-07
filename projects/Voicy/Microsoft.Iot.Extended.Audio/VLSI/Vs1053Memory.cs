namespace Microsoft.Iot.Extended.Audio.VLSI
{
    public enum Vs1053_SPI
    {
        CMD_WRITE = 0x02,
        CMD_READ = 0x03
    }

    public enum Vs_SCI_MODE
    {
        SM_RESET = 0x04,
        SM_CANCEL = 0x10,
        SM_TESTS = 0x20,
        SM_SDINEW = 0x800,
        SM_ADPCM = 0x1000,
        SM_LINE1 = 0x4000
    }

    public enum Vs_REC_MODE
    {
        PCM_MODE_JOINTSTEREO = 0x00,
        PCM_MODE_DUALCHANNEL = 0x01,
        PCM_MODE_LEFTCHANNEL = 0x02,
        PCM_MODE_RIGHTCHANNEL = 0x03
    }

    public enum Vs_REC_ENC
    {
        PCM_ENC_ADPCM = 0x00,
        PCM_ENC_PCM = 0x04,
    }

    public enum Vs1053_REGISTERS : byte
    {
        /// <summary>
        /// Mode control
        /// R/W
        /// </summary>
        SCI_MODE = 0x00,

        /// <summary>
        /// Status of VS1053b
        /// R/W
        /// </summary>
        SCI_STATUS = 0x01,

        /// <summary>
        /// Built-in bass/treble control
        /// R/W
        /// </summary>
        SCI_BASS = 0x02,

        /// <summary>
        /// Clock freq + multiplier
        /// R/W
        /// </summary>
        SCI_CLOCKF = 0x03,

        /// <summary>
        /// Volume control
        /// R/W
        /// </summary>
         SCI_WRAM = 0x06,

        /// <summary>
        /// Volume control
        /// R/W
        /// </summary>
        SCI_WRAMADDR = 0x07,

        /// <summary>
        /// Stream header data 0
        /// R
        /// </summary>
        SCI_HDAT0 = 0x08,

        /// <summary>
        /// Stream header data 1
        /// R
        /// </summary>
        SCI_HDAT1 = 0x09,

        /// <summary>
        /// Volume control
        /// R/W
        /// </summary>
        SCI_AIADDR = 0x0A,

        /// <summary>
        /// Volume control
        /// R/W
        /// </summary>
        SCI_VOL = 0x0B,

        /// <summary>
        /// Application control register 0
        /// R/W
        /// </summary>
        SCI_AICTRL0 = 0x0C,

        /// <summary>
        /// Application control register 1
        /// R/W
        /// </summary>
        SCI_AICTRL1 = 0x0D,

        /// <summary>
        /// Application control register 2
        /// R/W
        /// </summary>
        SCI_AICTRL2 = 0x0E,

        /// <summary>
        /// Application control register 3
        /// R/W
        /// </summary>
        SCI_AICTRL3 = 0x0F
    }
}
