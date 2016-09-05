using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Media;

namespace AudioEx
{
    using Windows.Devices.Gpio;

    // Using the COM interface IMemoryBufferByteAccess allows us to access the underlying byte array in an AudioFrame
    [ComImport]
    [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    unsafe interface IMemoryBufferByteAccess
    {
        void GetBuffer(out byte* buffer, out uint capacity);
    }

    public sealed class AnalogInputDevice : IDisposable
    {
        public AnalogInputDevice()
        {
            var gpio = GpioController.GetDefault();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private unsafe AudioFrame GetFrame(uint samples, uint channels, uint bps)
        {
            var bufferSize = samples * (bps / 4) * channels;

            var frame = new AudioFrame(bufferSize);
            
            using (var buffer = frame.LockBuffer(AudioBufferAccessMode.Write))
            using (var reference = buffer.CreateReference())
            {
                byte* bufferBytePointer;
                uint capacity;

                ((IMemoryBufferByteAccess)reference).GetBuffer(out bufferBytePointer, out capacity);

                for (var i = 0; i < samples; i++)
                {

                }
            }

            return frame;
        }
    }
}
