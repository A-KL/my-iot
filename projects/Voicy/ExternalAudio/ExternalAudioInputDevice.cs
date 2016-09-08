namespace ExternalAudio
{
    using System;
    using System.Runtime.InteropServices;
    using Windows.Foundation;
    using Windows.Media;
    using Windows.Media.Audio;

    // We are initializing a COM interface for use within the namespace
    // This interface allows access to memory at the byte level which we need to populate audio data that is generated
    [ComImport]
    [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]

    unsafe interface IMemoryBufferByteAccess
    {
        void GetBuffer(out byte* buffer, out uint capacity);
    }

    public class ExternalAudioInputDevice
    {
        private readonly AudioFrameInputNode inputNode;

        //var audio = await Vs1053.CreateAsync();
        //await audio.InitRecordingAsync();

        internal ExternalAudioInputDevice(AudioFrameInputNode inputNode)
        {
            this.inputNode = inputNode;

            this.inputNode.Stop();

            this.inputNode.QuantumStarted += this.FrameInputDevice_QuantumStarted;

            this.inputNode.Start();
        }

        public void AddOutgoingConnection(IAudioNode node)
        {
            this.inputNode.AddOutgoingConnection(node);
        }

        private void FrameInputDevice_QuantumStarted(AudioFrameInputNode sender, FrameInputNodeQuantumStartedEventArgs args)
        {
            var numSamplesNeeded = (uint)args.RequiredSamples;

            if (numSamplesNeeded > 0)
            {
                var audioData = this.GetNextFrame(numSamplesNeeded);
                this.inputNode.AddFrame(audioData);
            }
        }

        private unsafe AudioFrame GetNextFrame(uint samples)
        {
            uint channels = 1;
            var bufferSize = samples * channels * sizeof(float);

            var frame = new AudioFrame(bufferSize);

            using (AudioBuffer buffer = frame.LockBuffer(AudioBufferAccessMode.Write))
            using (IMemoryBufferReference reference = buffer.CreateReference())
            {
                byte* dataInBytes;
                uint capacityInBytes;

                // Get the buffer from the AudioFrame
                ((IMemoryBufferByteAccess)reference).GetBuffer(out dataInBytes, out capacityInBytes);

                // Cast to float since the data we are generating is float
                var dataInFloat = (float*)dataInBytes;

                float freq = 1000; // choosing to generate frequency of 1kHz
                float amplitude = 0.3f;

                var sampleRate = (int)this.inputNode.EncodingProperties.SampleRate;
                var sampleIncrement = (freq * (Math.PI * 2)) / sampleRate;

                // Generate a 1kHz sine wave and populate the values in the memory buffer
                for (int i = 0; i < samples; i++)
                {
                    double sinValue = amplitude * Math.Sin(theta);
                    dataInFloat[i] = (float)sinValue;
                    theta += sampleIncrement;
                }
            }

            return frame;
        }

        public double theta = 0;
    }
}
