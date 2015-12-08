namespace VideoCameraStreamer.ViewModels
{
    using System;
    using GalaSoft.MvvmLight;
    using Models;
    using Windows.Media.Capture;

    /// <summary>
    /// The main view model.
    /// </summary>
    public class MainViewModel: ViewModelBase
    {
        /// <summary>
        /// The capture.
        /// </summary>
        private MediaCapture capture;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
           // this.Initialize();
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        private async void Initialize()
        {
            var camera = new CameraModule();

            await camera.InitializeAsync();
            

            this.MediaCapture = camera.Source;

            await camera.Source.StartPreviewAsync();
        }

        /// <summary>
        /// Gets or sets the media capture.
        /// </summary>
        public MediaCapture MediaCapture
        {
            get
            {
                return this.capture;                
            }

            set
            {
                this.Set(() => this.MediaCapture, ref this.capture, value);
            }
        }
    }
}
