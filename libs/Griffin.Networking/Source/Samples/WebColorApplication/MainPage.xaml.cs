namespace WebColorApplication
{
    using System.Net;
    using System.Reflection;
    using Windows.UI.Core;
    using Windows.UI.Xaml.Navigation;
    using Griffin.Networking.Messaging;
    using Griffin.Networking.Protocol.Http;
    using Griffin.Networking.Web;
    using Griffin.Networking.Web.Listeners;
    using WebColorApplication.ViewModel;

    public sealed partial class MainPage
    {
        private const string DefaultPage = "index.html";

        public MainViewModel Vm => (MainViewModel)DataContext;

        public MainPage()
        {
            InitializeComponent();

            var assembly = this.GetType().GetTypeInfo().Assembly;

            var settings = new WebServiceSettings
            {
                DefaultPath = DefaultPage
            };
            
            //UseStaticFiles
            settings.Listeners.Add(new FileSystemListener("/", "wwwroot"));
            settings.Listeners.Add(new WebApiListener(assembly)); // use attribute routing

            var server = new MessagingServer(
                new WebServiceFactory(settings),
                new MessagingServerConfiguration(new HttpMessageFactory()));

            server.Start(new IPEndPoint(new IPAddress(new byte[] { 192, 168, 1, 12 }), 8000));

            //SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManagerBackRequested;

            //Loaded += (s, e) =>
            //{
            //    Vm.RunClock();
            //};
        }

        private void SystemNavigationManagerBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                e.Handled = true;
                Frame.GoBack();
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Vm.StopClock();
            base.OnNavigatingFrom(e);
        }
    }
}
