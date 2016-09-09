namespace Microsoft.Iot.Extended.Audio.VLSI
{
    public static class VsPatches
    {
    /* VS1053b IMA ADPCM Encoder Fix
        When IMA encoding mode is started, monitoring works, but encoded data transfer never starts.
        This patch fixes the problem.
        Version: 1.00
        Modified: 2008-05-23
        Devices: VS1053b
        Download: vs1053b-imafix100.zip*/

        public static readonly ushort[] IMA_ADPCM_Encoder_Fix =
        {
            0x0007, 0x0001, 0x8010, 0x0006, 0x001c, 0x3e12, 0xb817, 0x3e14, /*    0 */
            0xf812, 0x3e01, 0xb811, 0x0007, 0x9717, 0x0020, 0xffd2, 0x0030, /*    8 */
            0x11d1, 0x3111, 0x8024, 0x3704, 0xc024, 0x3b81, 0x8024, 0x3101, /*   10 */
            0x8024, 0x3b81, 0x8024, 0x3f04, 0xc024, 0x2808, 0x4800, 0x36f1, /*   18 */
            0x9811, 0x0007, 0x0001, 0x8028, 0x0006, 0x0002, 0x2a00, 0x040e
        };
    }
}