using System;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace WebServer.Service
{
    using Windows.ApplicationModel.AppService;
    using Windows.ApplicationModel.Background;

    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral serviceDeferral;
        private AppServiceConnection appServiceConnection;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Associate a cancellation handler with the background task. 
            taskInstance.Canceled += this.OnCanceled;

            // Get the deferral object from the task instance
            this.serviceDeferral = taskInstance.GetDeferral();

            var appService = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            if (appService == null || appService.Name != "WebServer.Service")
            {
                return;
            }

            this.appServiceConnection = appService.AppServiceConnection;
            this.appServiceConnection.RequestReceived += this.OnRequestReceived;
        }

        private async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var message = args.Request.Message;
            var command = (string) message["Command"];

            switch (command)
            {
                case "Initialize":
                    {
                        var messageDeferral = args.GetDeferral();
                        //Set a result to return to the caller
                        var returnMessage = new ValueSet();
                        HttpServer server = new HttpServer(8000, appServiceConnection);
                        IAsyncAction asyncAction = Windows.System.Threading.ThreadPool.RunAsync(
                            (workItem) =>
                            {
                                server.StartServer();
                            });
                        returnMessage.Add("Status", "Success");
                        var responseStatus = await args.Request.SendResponseAsync(returnMessage);
                        messageDeferral.Complete();
                        break;
                    }

                case "Quit":
                    {
                        //Service was asked to quit. Give us service deferral
                        //so platform can terminate the background task
                        serviceDeferral.Complete();
                        break;
                    }
            }
        }

        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            //Clean up and get ready to exit
        }
    }
}
