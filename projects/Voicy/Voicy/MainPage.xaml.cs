﻿namespace Voicy
{
    using AudioEffects;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.Devices.Enumeration;
    using Windows.Devices.Gpio;
    using Windows.Foundation.Collections;
    using Windows.Media.Audio;
    using Windows.Media.Effects;
    using Windows.Media.MediaProperties;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Microsoft.IoT.DeviceCore;
    using Microsoft.IoT.DeviceCore.Input;
    using Microsoft.IoT.Devices.Input;

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

        private GpioController gpioController;
        private RotaryEncoder rotary;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
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
                DesiredRenderDeviceAudioProcessing = Windows.Media.AudioProcessing.Raw,
                EncodingProperties = AudioEncodingProperties.CreatePcm(8000, 1, 16)
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


            //var frameInputDevice = this.graph.CreateExternalAudioDevice();

            //await frameInputDevice.Init();

            //frameInputDevice.AddOutgoingConnection(this.deviceOutputNode);

            var inputDeviceResult =
                await this.graph.CreateDeviceInputNodeAsync(Windows.Media.Capture.MediaCategory.Speech);

            if (inputDeviceResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                return;
            }

            this.deviceInputNode = inputDeviceResult.DeviceInputNode;

            this.deviceInputNode.AddOutgoingConnection(this.deviceOutputNode);

            // Effects

            this.CreatePitchEffect();

            this.InitializeEncoder();

            // Done
            this.graph.Start();

            this.pitchToggleSwitch.IsOn = true;
        }

        private void InitializeEncoder()
        {
            this.gpioController = GpioController.GetDefault();
            if (this.gpioController == null)
            {
                /// ("GPIO Controller not found!");
                return;
            }

            // Create rotary
            this.rotary = new RotaryEncoder()
            {
                // ButtonPin = gpioController.OpenPin(26),
                ClockPin = this.gpioController.OpenPin(22),
                DirectionPin = this.gpioController.OpenPin(13),
            };

            // Subscribe to events
            //this.rotary.Click += Rotary_Click;
            this.rotary.Rotated += Rotary_Rotated;
        }

        private void Rotary_Rotated(IRotaryEncoder sender, RotaryEncoderRotatedEventArgs args)
        {
            if (this.pitch == null)
            {
                return;
            }

            var value = (double)this.pitch.Properties["Value"];

            if (args.Direction == RotationDirection.Clockwise)
            {
                if (value + 0.1 > 2.0)
                {
                    return;
                }
                value += 0.1;
            }
            else if (args.Direction == RotationDirection.Counterclockwise)
            {
                if (value - 0.1 < 0.5)
                {
                    return;
                }
                value -= 0.1;
            }
            
            this.pitch.Properties["Value"] = value;
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
            // this.deviceOutputNode.DisableEffectsByDefinition(this.equalizer);
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
            //this.deviceOutputNode.DisableEffectsByDefinition(this.reverb);
        }

        private async Task<DeviceInformation> GetDefaultDeviceAsync(DeviceClass deviceClass)
        {
            var audioDevices = await DeviceInformation.FindAllAsync(deviceClass);

            var devs = audioDevices.ToList();

            var audioDevice = devs[devs.Count - 1];

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