namespace WebServer.Shared
{
    using System;
    using System.Threading.Tasks;
    using Windows.UI.Core;
    using Windows.Foundation.Collections;
    using Windows.ApplicationModel.AppService;

    public class WebServerConnector :  IDisposable
    {
        private readonly AppServiceConnection serviceConnection;

        public WebServerConnector()
        {
            this.serviceConnection = new AppServiceConnection
            {
                PackageFamilyName = "WebServer.Service_jchzd8696eq26",
                AppServiceName = "WebServer.Service"
            };
        }

        public async Task Init()
        {
            // Send a initialize request 
            var res = await this.serviceConnection.OpenAsync();

            if (res == AppServiceConnectionStatus.Success)
            {
                var message = new ValueSet { {"Command", "Initialize"} };
                
                var response = await this.serviceConnection.SendMessageAsync(message);

                if (response.Status != AppServiceResponseStatus.Success)
                {
                    throw new Exception("Failed to send message");
                }

                this.serviceConnection.RequestReceived += this.OnMessageReceived;
            }
        }

        private void OnMessageReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var message = args.Request.Message;
            string newState = message["State"] as string;
            switch (newState)
            {
                case "On":
                    {

                        break;
                    }
                case "Off":
                    {

                        break;
                    }
                case "Unspecified":
                default:
                    {
                        // Do nothing 
                        break;
                    }
            }
        }

        public void Dispose()
        {
            this.serviceConnection.Dispose();
        }
    }
}
