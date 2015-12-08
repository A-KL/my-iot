namespace VideoCameraStreamer.ViewModels
{
    using System.Diagnostics.CodeAnalysis;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Ioc;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The view model locator.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes static members of the <see cref="ViewModelLocator"/> class.
        /// </summary>
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (!ViewModelBase.IsInDesignModeStatic)
            {
                // SimpleIoc.Default.Register<IRaceDataService>(() => new CsvRaceDataService(Settings.RacersCount, Settings.LapsCount, Settings.DataFilePath));
            }

            SimpleIoc.Default.Register<MainViewModel>();
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", 
            "CA1822:MarkMembersAsStatic", 
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}
