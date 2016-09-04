using AudioEffects;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation.Collections;
using Windows.Media.Audio;
using Windows.Media.Effects;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Voicy
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private EqualizerEffectDefinition equalizer;
        private ReverbEffectDefinition reverb;

        private AudioGraph graph;
        private AudioDeviceInputNode deviceInputNode;
        private AudioDeviceOutputNode deviceOutputNode;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Devices

            var outputDevice = await GetDefaultDeviceAsync(DeviceClass.AudioRender);
            if (outputDevice == null)
            {
                return;
            }

            // Graph

            var settings = new AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media);
            
            settings.PrimaryRenderDevice = outputDevice;

            settings.EncodingProperties = AudioEncodingProperties.CreatePcm(24000, 1, 16);
            
            var result = await AudioGraph.CreateAsync(settings);

            if (result.Status != AudioGraphCreationStatus.Success)
            {
                return;
            }

            this.graph = result.Graph;

            // Output

            var outputDeviceResult = await graph.CreateDeviceOutputNodeAsync();

            if (outputDeviceResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                return;
            }

            this.deviceOutputNode = outputDeviceResult.DeviceOutputNode;

            // Input
            var inputDeviceResult = await graph.CreateDeviceInputNodeAsync(Windows.Media.Capture.MediaCategory.Other);
            if (inputDeviceResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                return;
            }

            this.deviceInputNode = inputDeviceResult.DeviceInputNode;
            
            this.deviceInputNode.AddOutgoingConnection(this.deviceOutputNode);

            // Effects

            // Reverb
            //this.CreateReverbEffect(graph);

            // outputNode.EffectDefinitions.Add(this.reverb);

            // Eualizer
            //this.CreateEqEffect(graph);

            //outputNode.EffectDefinitions.Add(equalizer);

            // Custom
            // Create a property set and add a property/value pair
            PropertySet properties = new PropertySet();

            properties.Add("Value", 0.8f);

            var pitch = new AudioEffectDefinition(typeof(PitchAudioEffect).FullName, properties);

            this.deviceOutputNode.EffectDefinitions.Add(pitch);

            // Done
            graph.Start();
        }

        private void slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            return;

            if (this.equalizer == null)
            {
                return;
            }

            var slider = sender as Slider;

            var value = ConvertRange(slider.Value);

            if (slider == this.slider0)
            {
                this.equalizer.Bands[0].Gain = value;
            }
            else if (slider == this.slider1)
            {
                this.equalizer.Bands[1].Gain = value;
            }
            else if (slider == this.slider2)
            {
                this.equalizer.Bands[2].Gain = value;
            }
            else if (slider == this.slider3)
            {
                this.equalizer.Bands[3].Gain = value;
            }
        }

        private void CreateEqEffect(AudioGraph graph)
        {
            // See the MSDN page for parameter explanations
            // https://msdn.microsoft.com/en-us/library/windows/desktop/microsoft.directx_sdk.xapofx.fxeq_parameters(v=vs.85).aspx
            this.equalizer = new EqualizerEffectDefinition(graph);
            this.equalizer.Bands[0].FrequencyCenter = 20.0f;
            this.equalizer.Bands[0].Gain = 4.033f;
            this.equalizer.Bands[0].Bandwidth = 1.5f;

            this.equalizer.Bands[1].FrequencyCenter = 200.0f;
            this.equalizer.Bands[1].Gain = 1.6888f;
            this.equalizer.Bands[1].Bandwidth = 1.5f;

            this.equalizer.Bands[2].FrequencyCenter = 10000.0f;
            this.equalizer.Bands[2].Gain = 0.128;
            this.equalizer.Bands[2].Bandwidth = 1.5f;

            this.equalizer.Bands[3].FrequencyCenter = 20000.0f;
            this.equalizer.Bands[3].Gain = 0.128;
            this.equalizer.Bands[3].Bandwidth = 2.0f;
        }

        private void CreateReverbEffect(AudioGraph graph)
        {
            // Create reverb effect
            this.reverb = new ReverbEffectDefinition(graph);

            // See the MSDN page for parameter explanations
            // https://msdn.microsoft.com/en-us/library/windows/desktop/microsoft.directx_sdk.xaudio2.xaudio2fx_reverb_parameters(v=vs.85).aspx
            this.reverb.WetDryMix = 50;
            this.reverb.ReflectionsDelay = 120;
            this.reverb.ReverbDelay = 30;
            this.reverb.RearDelay = 3;
            this.reverb.DecayTime = 0.5;
        }

        private async Task<DeviceInformation> GetDefaultDeviceAsync(DeviceClass deviceClass)
        {
            var audioDevices = await DeviceInformation.FindAllAsync(deviceClass);

            var audioDevice = audioDevices.FirstOrDefault(d => d.IsEnabled);

            return audioDevice;
        }

        // Mapping the 0-100 scale of the slider to a value between the min and max gain
        private double ConvertRange(double value)
        {
            // These are the same values as the ones in xapofx.h
            const double fxeq_min_gain = 0.126;
            const double fxeq_max_gain = 7.94;

            double scale = (fxeq_max_gain - fxeq_min_gain) / 100;
            return (fxeq_min_gain + ((value) * scale));
        }
    }
}