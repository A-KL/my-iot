using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Effects;
using Windows.Media.MediaProperties;

namespace AudioEffects
{
    // Using the COM interface IMemoryBufferByteAccess allows us to access the underlying byte array in an AudioFrame
    [ComImport]
    [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    unsafe interface IMemoryBufferByteAccess
    {
        void GetBuffer(out byte* buffer, out uint capacity);
    }

    public sealed class PitchAudioEffect : IBasicAudioEffect
    {
        private List<AudioEncodingProperties> supportedEncodingProperties;
        private AudioEncodingProperties currentEncodingProperties;
        private IPropertySet propertySet;

        public PitchAudioEffect()
        {
            this.supportedEncodingProperties = new List<AudioEncodingProperties>();

            // Support 44.1kHz and 48kHz mono float
            AudioEncodingProperties encodingProps1 = AudioEncodingProperties.CreatePcm(44100, 1, 32);
            encodingProps1.Subtype = MediaEncodingSubtypes.Float;

            AudioEncodingProperties encodingProps2 = AudioEncodingProperties.CreatePcm(48000, 1, 32);
            encodingProps2.Subtype = MediaEncodingSubtypes.Float;

            AudioEncodingProperties encodingProps3 = AudioEncodingProperties.CreatePcm(16000, 1, 16);
            encodingProps3.Subtype = MediaEncodingSubtypes.Float;
            supportedEncodingProperties.Add(encodingProps1);
            supportedEncodingProperties.Add(encodingProps2);
            supportedEncodingProperties.Add(encodingProps3);
         }

        private float Value
        {
            get { return (float)propertySet["Value"]; }
        }

        public IReadOnlyList<AudioEncodingProperties> SupportedEncodingProperties
        {
            get
            {
                return this.supportedEncodingProperties;
            }
        }

        public bool UseInputFrameForOutput
        {
            get
            {
                return false;
            }
        }


        public void Close(MediaEffectClosedReason reason)
        {

        }

        public void DiscardQueuedFrames()
        {

        }

        public void SetEncodingProperties(AudioEncodingProperties encodingProperties)
        {
            currentEncodingProperties = encodingProperties;
        }

        public void SetProperties(IPropertySet configuration)
        {
            this.propertySet = configuration;
        }

        private Stopwatch sw = new Stopwatch();

        unsafe public void ProcessFrame(ProcessAudioFrameContext context)
        {
            AudioFrame inputFrame = context.InputFrame;
            AudioFrame outputFrame = context.OutputFrame;

            using (AudioBuffer inputBuffer = inputFrame.LockBuffer(AudioBufferAccessMode.Read),
                                outputBuffer = outputFrame.LockBuffer(AudioBufferAccessMode.Write))
            using (IMemoryBufferReference inputReference = inputBuffer.CreateReference(),
                                            outputReference = outputBuffer.CreateReference())
            {
                byte* inputDataInBytes;
                byte* outputDataInBytes;
                uint inputCapacity;
                uint outputCapacity;

                ((IMemoryBufferByteAccess)inputReference).GetBuffer(out inputDataInBytes, out inputCapacity);
                ((IMemoryBufferByteAccess)outputReference).GetBuffer(out outputDataInBytes, out outputCapacity);

                float* inputDataInFloat = (float*)inputDataInBytes;
                float* outputDataInFloat = (float*)outputDataInBytes;

                int dataInFloatLength = (int)inputBuffer.Length / sizeof(float);

               // sw.Restart();

                PitchShifter.PitchShift(this.Value, dataInFloatLength, (long)512, (long)10, this.currentEncodingProperties.SampleRate, inputDataInFloat, outputDataInFloat);

                //sw.Stop();

                //Debug.WriteLine(sw.ElapsedMilliseconds);

            }

        }        
    }
}
