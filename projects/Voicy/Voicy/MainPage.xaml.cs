using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Audio;
using Windows.Media.MediaProperties;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Voicy
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private EqualizerEffectDefinition equalizer;

        private const double BandGainMax = 7.94;
        private const double BandGainMin = 0.126;
        private const double BandGainVal = 1.0;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async Task<DeviceInformation> GetDefaultDeviceAsync(DeviceClass deviceClass)
        {
            var audioDevices = await DeviceInformation.FindAllAsync(deviceClass);

            var audioDevice = audioDevices.FirstOrDefault(d => d.IsEnabled);

            return audioDevice;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Devices

            var outputDevice = await GetDefaultDeviceAsync(DeviceClass.AudioRender);

            // var inputDevice = await GetDefaultDeviceAsync(DeviceClass.AudioCapture);

            if (outputDevice == null)
            {
                return;
            }

            // Graph

            var settings = new AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media);
           // settings.QuantumSizeSelectionMode = QuantumSizeSelectionMode.LowestLatency;
            settings.DesiredRenderDeviceAudioProcessing = Windows.Media.AudioProcessing.Default;
            settings.QuantumSizeSelectionMode = QuantumSizeSelectionMode.ClosestToDesired;
            settings.PrimaryRenderDevice = outputDevice;

            var result = await AudioGraph.CreateAsync(settings);

            if (result.Status != AudioGraphCreationStatus.Success)
            {
                return;
            }

            var graph = result.Graph;

            // Output

            var outputDeviceResult = await graph.CreateDeviceOutputNodeAsync();

            if (outputDeviceResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                return;
            }

            var outputNode = outputDeviceResult.DeviceOutputNode;

            // Input

            var inputDeviceResult = await graph.CreateDeviceInputNodeAsync(Windows.Media.Capture.MediaCategory.Other);

            if (inputDeviceResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                return;
            }

            var inputNode = inputDeviceResult.DeviceInputNode;

            inputNode.AddOutgoingConnection(outputNode);

            // See the MSDN page for parameter explanations
            // https://msdn.microsoft.com/en-us/library/windows/desktop/microsoft.directx_sdk.xapofx.fxeq_parameters(v=vs.85).aspx
            equalizer = new EqualizerEffectDefinition(graph);
            equalizer.Bands[0].FrequencyCenter = 20.0f;
            equalizer.Bands[0].Gain = 7.94f;
            equalizer.Bands[0].Bandwidth = 1.5f;

            equalizer.Bands[1].FrequencyCenter = 100.0f;
            equalizer.Bands[1].Gain = 7.94f;
            equalizer.Bands[1].Bandwidth = 1.5f;

            equalizer.Bands[2].FrequencyCenter = 5000.0f;
            equalizer.Bands[2].Gain = 0.126;
            equalizer.Bands[2].Bandwidth = 1.5f;

            equalizer.Bands[3].FrequencyCenter = 20000.0f;
            equalizer.Bands[3].Gain = 0.126f;
            equalizer.Bands[3].Bandwidth = 2.0f;

            outputNode.EffectDefinitions.Add(equalizer);


            // create echo effect
           var echoEffectDefinition = new EchoEffectDefinition(graph);

            // See the MSDN page for parameter explanations
            // http://msdn.microsoft.com/en-us/library/windows/desktop/microsoft.directx_sdk.xapofx.fxecho_parameters(v=vs.85).aspx
            echoEffectDefinition.WetDryMix = 0.2f;
            echoEffectDefinition.Feedback = 0.7f;
            echoEffectDefinition.Delay = 40.0f; // 500.0f

           // outputNode.EffectDefinitions.Add(echoEffectDefinition);

            // Create reverb effect
           var reverbEffectDefinition = new ReverbEffectDefinition(graph);

            // See the MSDN page for parameter explanations
            // https://msdn.microsoft.com/en-us/library/windows/desktop/microsoft.directx_sdk.xaudio2.xaudio2fx_reverb_parameters(v=vs.85).aspx
            reverbEffectDefinition.WetDryMix = 50;
            reverbEffectDefinition.ReflectionsDelay = 120;
            reverbEffectDefinition.ReverbDelay = 30;
            reverbEffectDefinition.RearDelay = 3;
            reverbEffectDefinition.DecayTime = 0.3; //2;

            outputNode.EffectDefinitions.Add(reverbEffectDefinition);

            // Done!
            graph.Start();
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


        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
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
    }
}
