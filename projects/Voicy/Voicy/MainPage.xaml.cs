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

            if ( outputDevice == null)
            {
                return;
            }

            // Graph

            var settings = new AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Speech);

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

            var inputDeviceResult = await graph.CreateDeviceInputNodeAsync(Windows.Media.Capture.MediaCategory.Speech);

            if (inputDeviceResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                return;
            }

            var inputNode = inputDeviceResult.DeviceInputNode;

            EchoEffectDefinition echoEffectDefinition = new EchoEffectDefinition(graph);
            echoEffectDefinition.Delay = 100.0f;
            echoEffectDefinition.WetDryMix = 0.7f;
            echoEffectDefinition.Feedback = 0.5f;
            //inputNode.EffectDefinitions.Add(echoEffectDefinition);

            ReverbEffectDefinition reverbEffectDefinition = new ReverbEffectDefinition(graph);
            reverbEffectDefinition.DecayTime = 0.05;
            //reverbEffectDefinition.// = 0.005;
            inputNode.EffectDefinitions.Add(reverbEffectDefinition);
            
            EqualizerEffectDefinition equalizer = new EqualizerEffectDefinition(graph);
            var b1 = equalizer.Bands[0];
            var b2 = equalizer.Bands[1];
            var b3 = equalizer.Bands[2];
            var b4 = equalizer.Bands[3];
            
            b1.Bandwidth = 1;
            b1.Gain = 6;
            b1.FrequencyCenter = 30;

            inputNode.EffectDefinitions.Add(equalizer);

            inputNode.AddOutgoingConnection(outputNode);

            // Done!

            graph.Start();
        }
    }
}
