namespace ExternalAudio
{
    using Windows.Media.Audio;
    using Windows.Media.MediaProperties;

    public static class AudioGraphExtensions
    {
        public static ExternalAudioInputDevice CreateExternalAudioDevice(this AudioGraph graph)
        {
            return new ExternalAudioInputDevice(graph.CreateFrameInputNode());
        }

        public static ExternalAudioInputDevice CreateExternalAudioDevice(this AudioGraph graph, AudioEncodingProperties properties)
        {
            return new ExternalAudioInputDevice(graph.CreateFrameInputNode(properties));
        }
    }
}
