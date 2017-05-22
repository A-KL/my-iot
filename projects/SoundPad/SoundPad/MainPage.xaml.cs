﻿namespace SoundPad
{
    using System;
    using System.Threading.Tasks;

    using Windows.Storage;
    using Windows.UI;
    using Windows.UI.Xaml.Media;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static MediaElement[,] Sounds = new MediaElement[3, 3];

        public MainPage()
        {
            this.InitializeComponent();

            LoadSounds();
        }

        private static async Task LoadSounds()
        {
            for (var i = 0; i < Sounds.Rank; i++)
            {
                for (var j = 0; j < Sounds.Rank; j++)
                {
                    //Sounds[i, j] = await LoadSoundFile($"Sound_{i}_{j}.wav");
                    Sounds[i, j] = await LoadSoundFile("Rhythm-machine-loop.wav");
                }
            }
        }

        private static async Task<MediaElement> LoadSoundFile(string filePath)
        {
            var media = new MediaElement
            {
                AutoPlay = false
            };

            var folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Sounds");

            var file = await folder.GetFileAsync(filePath);

            var stream = await file.OpenAsync(FileAccessMode.Read);

            media.SetSource(stream, file.ContentType);

            return media;
        }

        private void OnToggleButtonClick(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;

            var col = Grid.GetColumn(button);
            var row = Grid.GetRow(button);

            var sound = Sounds[col, row];

            if (sound == null)
            {
                button.Background = new SolidColorBrush(Colors.Red);

                return;
            }

            sound.Stop();
            sound.Play();
        }

        private void OnToggleButtonCompleted(object sender, RoutedEventArgs e)
        {
    
        }
    }
}