using AudioEffects;
using Microsoft.Iot.Extended.Audio.VLSI;
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
        private AudioEffectDefinition pitch;

        private AudioGraph graph;
        private AudioDeviceInputNode deviceInputNode;
        private AudioDeviceOutputNode deviceOutputNode;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var audio = await Vs1053.CreateAsync();
            await audio.InitRecordingAsync();
            
            // Devices

            var outputDevice = await this.GetDefaultDeviceAsync(DeviceClass.AudioRender);
            if (outputDevice == null)
            {
                return;
            }

            // Graph

            var settings = new AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media)
            {
                PrimaryRenderDevice = outputDevice,
                QuantumSizeSelectionMode = QuantumSizeSelectionMode.LowestLatency,
                DesiredRenderDeviceAudioProcessing = Windows.Media.AudioProcessing.Raw,
                EncodingProperties = AudioEncodingProperties.CreatePcm(16000, 1, 16)
            };

            var result = await AudioGraph.CreateAsync(settings);

            if (result.Status != AudioGraphCreationStatus.Success)
            {
                return;
            }

            this.graph = result.Graph;

            // Output

            var outputDeviceResult = await this.graph.CreateDeviceOutputNodeAsync();

            if (outputDeviceResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                return;
            }

            this.deviceOutputNode = outputDeviceResult.DeviceOutputNode;

            // Input

            var mic = await this.GetDefaultDeviceAsync(DeviceClass.AudioCapture);

            var inputDeviceResult = await this.graph.CreateDeviceInputNodeAsync(Windows.Media.Capture.MediaCategory.Speech, graph.EncodingProperties, mic);
            if (inputDeviceResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                return;
            }

            this.deviceInputNode = inputDeviceResult.DeviceInputNode;

            this.deviceInputNode.AddOutgoingConnection(this.deviceOutputNode);

            // Effects

            //this.CreateReverbEffect();

            // this.CreateEqEffect();

            //this.CreatePitchEffect();

            // Done
            this.graph.Start();

           // this.pitchToggleSwitch.IsOn = true;
        }

        private void PitchSlider_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (this.pitch == null)
            {
                return;
            }

            this.pitch.Properties["Value"] = (float)e.NewValue;
        }

        private void slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (this.equalizer == null)
            {
                return;
            }

            var slider = (Slider)sender;

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

        private void CreatePitchEffect()
        {
            var properties = new PropertySet { { "Value", 0.8f } };

            this.pitch = new AudioEffectDefinition(typeof(PitchAudioEffect).FullName, properties);

            this.deviceOutputNode.EffectDefinitions.Add(this.pitch);
            this.deviceOutputNode.DisableEffectsByDefinition(this.pitch);
        }

        private void CreateEqEffect()
        {
            // See the MSDN page for parameter explanations
            // https://msdn.microsoft.com/en-us/library/windows/desktop/microsoft.directx_sdk.xapofx.fxeq_parameters(v=vs.85).aspx
            this.equalizer = new EqualizerEffectDefinition(this.graph);
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

            this.deviceOutputNode.EffectDefinitions.Add(this.equalizer);
            this.deviceOutputNode.DisableEffectsByDefinition(this.equalizer);
        }

        private void CreateReverbEffect()
        {
            // See the MSDN page for parameter explanations
            // https://msdn.microsoft.com/en-us/library/windows/desktop/microsoft.directx_sdk.xaudio2.xaudio2fx_reverb_parameters(v=vs.85).aspx
            this.reverb = new ReverbEffectDefinition(this.graph)
            {
                WetDryMix = 50,
                ReflectionsDelay = 120,
                ReverbDelay = 30,
                RearDelay = 3,
                DecayTime = 0.5
            };

            this.deviceOutputNode.EffectDefinitions.Add(this.reverb);
            this.deviceOutputNode.DisableEffectsByDefinition(this.reverb);
        }

        private async Task<DeviceInformation> GetDefaultDeviceAsync(DeviceClass deviceClass)
        {
            var audioDevices = await DeviceInformation.FindAllAsync(deviceClass);

            var audioDevice = audioDevices.FirstOrDefault(d => d.IsEnabled);

            return audioDevice;
        }

        // Mapping the 0-100 scale of the slider to a value between the min and max gain
        private static double ConvertRange(double value)
        {
            // These are the same values as the ones in xapofx.h
            const double fxeq_min_gain = 0.126;
            const double fxeq_max_gain = 7.94;

            double scale = (fxeq_max_gain - fxeq_min_gain) / 100;
            return (fxeq_min_gain + ((value) * scale));
        }

        private void PitchToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
        {
            if (this.pitch == null)
            {
                return;
            }

            var sw = (ToggleSwitch)sender;

            if (sw == this.pitchToggleSwitch)
            {
                if (sw.IsOn)
                {
                    this.deviceOutputNode.EnableEffectsByDefinition(this.pitch);
                }
                else
                {
                    this.deviceOutputNode.DisableEffectsByDefinition(this.pitch);
                }
            }
        }
    }
}