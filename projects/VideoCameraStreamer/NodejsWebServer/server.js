var http = require('http');
var uwp = require('uwp');
//var addon = require("./MemoryStatusAddon");

// Camera
uwp.projectNamespace("Windows.Media.Capture");

var settings = Windows.Media.Capture.MediaCaptureInitializationSettings();
settings.VideoDeviceId = 0;

var mediaCapture = Windows.Media.Capture.MediaCapture();
mediaCapture.InitializeAsync(settings);

var videoFrame = new Windows.Media.Capture.VideoFrame(BitmapPixelFormat.Bgra8, 640, 480);
mediaCapture.GetPreviewFrameAsync(videoFrame);

// GPIO
uwp.projectNamespace("Windows.Devices");
var gpioController = Windows.Devices.Gpio.GpioController.getDefault();

var pin = gpioController.openPin(6);
pin.setDriveMode(Windows.Devices.Gpio.GpioPinDriveMode.output)

var currentValue = Windows.Devices.Gpio.GpioPinValue.high;
pin.write(currentValue);

http.createServer(function (req, res)
{
    if (currentValue == Windows.Devices.Gpio.GpioPinValue.high)
    {
        currentValue = Windows.Devices.Gpio.GpioPinValue.low;
    }
    else
    {
        currentValue = Windows.Devices.Gpio.GpioPinValue.high;
    }

    pin.write(currentValue);
    res.writeHead(200, { 'Content-Type': 'text/plain' });
    res.end('LED value: ' + currentValue + '\n');

	res.writeHead(200, { 'Content-Type': 'text/plain' });
	res.end('Hello World\n');

	//var memObj = addon.GlobalMemoryStatusEx();
	//res.writeHead(200, { 'Content-Type': 'text/plain' });
	//res.write('*************************************************\n');
	//res.write('Percent of memory in use: ' + memObj.load + '\n');
	//res.write('KB of physical memory: ' + memObj.physKb + '\n');
	//res.write('KB of free physical memory: ' + memObj.freePhysKb + '\n');
	//res.write('KB of paging file: ' + memObj.pageKb + '\n');
	//res.write('KB of free paging file: ' + memObj.freePageKb + '\n');
	//res.write('KB of virtual memory: ' + memObj.virtualKb + '\n');
	//res.write('KB of free virtual memory: ' + memObj.freeVirtualKb + '\n');
	//res.write('KB of free extended memory: ' + memObj.freeExtKb + '\n');
	//res.end('*************************************************\n');

}).listen(1337);
