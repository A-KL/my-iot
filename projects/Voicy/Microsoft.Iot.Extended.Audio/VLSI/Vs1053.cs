using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Iot.Extended.Audio.VLSI
{
    public class Vs1053 : AudioDevice
    {
        public byte[] Capture()
        {
            this.WriteRegister((byte)Vs1053_REGISTERS.SCI_MODE, 
                ReadVS10xxRegister(SCI_MODE) | SM_RESET | SM_ADPCM | SM_LINE1);

            this.WriteRegister((byte)Vs1053_REGISTERS.SCI_AICTRL0, 16000);
            this.WriteRegister((byte)Vs1053_REGISTERS.SCI_AICTRL1, 16000);
            this.WriteRegister((byte)Vs1053_REGISTERS.SCI_AICTRL2, 16000);
            this.WriteRegister((byte)Vs1053_REGISTERS.SCI_AICTRL0, 16000);
        }
    }
}
