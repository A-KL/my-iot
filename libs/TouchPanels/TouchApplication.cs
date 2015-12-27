namespace TouchPanels
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Automation.Peers;
    using Windows.UI.Xaml.Automation.Provider;
    using Windows.UI.Xaml.Media;
    using TouchPanels.Devices;

    public abstract class TouchApplication : Application
    {
        private const string CalibrationFilename = "TSC2046";
        private Tsc2046 tsc2046;
        private TouchProcessor touch;
        private IScrollProvider currentScrollItem;
        private Point lastPosition = new Point(double.NaN, double.NaN);

        protected void UnInitializeTouch()
        {
            if (this.touch == null)
            {
                return;

            }

            //Unhooking from all the touch events, will automatically shut down the processor.
            //Remember to do this, or you view could be staying in memory.
            this.touch.PointerDown -= Processor_PointerDown;
            this.touch.PointerMoved -= Processor_PointerMoved;
            this.touch.PointerUp -= Processor_PointerUp;
        }

        protected async Task InitTouch()
        {
            this.tsc2046 = await Tsc2046.GetDefaultAsync();

            try
            {
                await tsc2046.LoadCalibrationAsync(CalibrationFilename);
            }
            catch (System.IO.FileNotFoundException)
            {
                await this.CalibrateTouch(); //Initiate calibration if we don't have a calibration on file
            }
            catch (UnauthorizedAccessException)
            {
                //No access to documents folder
                // await new Windows.UI.Popups.MessageDialog("Make sure the application manifest specifies access to the documents folder and declares the file type association for the calibration file.", "Configuration Error").ShowAsync();
                // throw;
                return;
            }

            //Load up the touch processor and listen for touch events
            this.touch = new TouchProcessor(tsc2046);
            this.touch.PointerDown += this.Processor_PointerDown;
            this.touch.PointerMoved += this.Processor_PointerMoved;
            this.touch.PointerUp += this.Processor_PointerUp;
        }

        private async Task CalibrateTouch()
        {
            //_isCalibrating = true;
            var calibration = await UI.LcdCalibrationView.CalibrateScreenAsync(tsc2046);
            //_isCalibrating = false;
            tsc2046.SetCalibration(calibration.A, calibration.B, calibration.C, calibration.D, calibration.E, calibration.F);
            try
            {
                await tsc2046.SaveCalibrationAsync(CalibrationFilename);
            }
            catch (Exception)
            {
                //Status.Text = ex.Message;
            }
        }

        private void Processor_PointerDown(object sender, PointerEventArgs e)
        {
            this.currentScrollItem = this.FindElementsToInvoke(Window.Current.Content, e.Position);
            this.lastPosition = e.Position;
        }

        private void Processor_PointerMoved(object sender, PointerEventArgs e)
        {
            if (this.currentScrollItem != null)
            {
                var dx = e.Position.X - lastPosition.X;
                var dy = e.Position.Y - lastPosition.Y;

                if (!currentScrollItem.HorizontallyScrollable)
                {
                    dx = 0;
                }

                if (!currentScrollItem.VerticallyScrollable)
                {
                    dy = 0;
                }

                var h = Windows.UI.Xaml.Automation.ScrollAmount.NoAmount;
                var v = Windows.UI.Xaml.Automation.ScrollAmount.NoAmount;

                if (dx < 0)
                {
                    h = Windows.UI.Xaml.Automation.ScrollAmount.SmallIncrement;
                }
                else if (dx > 0)
                {
                    h = Windows.UI.Xaml.Automation.ScrollAmount.SmallDecrement;
                }

                if (dy < 0)
                {
                    v = Windows.UI.Xaml.Automation.ScrollAmount.SmallIncrement;
                }
                else if (dy > 0)
                {
                    v = Windows.UI.Xaml.Automation.ScrollAmount.SmallDecrement;
                }

                currentScrollItem.Scroll(h, v);
            }
            lastPosition = e.Position;
        }
        private void Processor_PointerUp(object sender, PointerEventArgs e)
        {
            this.currentScrollItem = null;
        }

        private IScrollProvider FindElementsToInvoke(UIElement root, Point screenPosition)
        {
            //    if (_isCalibrating)
            //    {
            //        return null;
            //    }
            var elements = VisualTreeHelper.FindElementsInHostCoordinates(new Point(screenPosition.X, screenPosition.Y), root, false);
            //Search for buttons in the visual tree that we can invoke
            //If we can find an element button that implements the 'Invoke' automation pattern (usually buttons), we'll invoke it
            foreach (var e in elements.OfType<FrameworkElement>())
            {
                var element = e;
                object pattern = null;

                while (true)
                {
                    var peer = FrameworkElementAutomationPeer.FromElement(element);

                    if (peer != null)
                    {
                        pattern = peer.GetPattern(PatternInterface.Invoke);
                        if (pattern != null)
                        {
                            break;
                        }
                        pattern = peer.GetPattern(PatternInterface.Scroll);
                        if (pattern != null)
                        {
                            break;
                        }
                    }

                    var parent = VisualTreeHelper.GetParent(element);

                    if (parent is FrameworkElement)
                        element = parent as FrameworkElement;
                    else
                        break;
                }

                if (pattern != null)
                {
                    var p = pattern as IInvokeProvider;
                    p?.Invoke();
                    return pattern as IScrollProvider;
                }
            }
            return null;
        }
    }
}
