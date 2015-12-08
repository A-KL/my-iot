// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CameraModule.cs" company="">
//   
// </copyright>
// <summary>
//   The camera module.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;

namespace VideoCameraStreamer.Models
{
    /// <summary>
    /// The camera module.
    /// </summary>
    public class CameraModule : IDisposable
    {
        /// <summary>
        /// Media Capture object for the USB camera
        /// </summary>
        private MediaCapture mediaCapture;

        /// <summary>
        /// The discovery.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<IEnumerable<CameraModule>> Discovery()
        {
            var devices = await FindCameraDevice();

            return null;
        }

        /// <summary>
        /// Gets the source.
        /// </summary>
        public MediaCapture Source
        {
            get { return this.mediaCapture; }
        }

        /// <summary>
        /// The start.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Start()
        {
            try
            {
                await mediaCapture.StartPreviewAsync();
            }
            catch
            {
                Debug.WriteLine("UsbCamera: Failed to start camera preview stream");
                throw;
            }
        }

        /// <summary>
        /// Asynchronously initializes webcam feed
        /// </summary>
        /// <returns>
        /// Task object: True if camera is successfully initialized; false otherwise.
        /// </returns>
        public async Task<bool> InitializeAsync()
        {
            if (mediaCapture != null)
            {
                return false;
            }

            // Attempt to get attached webcam
            var cameraDevice = await FindCameraDevice();

            if (cameraDevice == null)
            {
                // No camera found, report the error and break out of initialization
                Debug.WriteLine("UsbCamera: No camera found!");

// isInitialized = false;
                return false;
            }

            // Creates MediaCapture initialization settings with foudnd webcam device
           // var settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id };
            this.mediaCapture = new MediaCapture();

            try
            {
                await mediaCapture.InitializeAsync();

// isInitialized = true;
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine("UsbCamera: UnauthorizedAccessException: " + ex + "Ensure webcam capability is added in the manifest.");
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("UsbCamera: Exception when initializing MediaCapture:" + ex);
                throw;
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.mediaCapture?.Dispose();
        }

        /// <summary>
        /// The find camera device.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private static async Task<DeviceInformation> FindCameraDevice()
        {
            // Get available devices for capturing pictures
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            return allVideoDevices.Count > 0 ? allVideoDevices[0] : null;
        }
    }
}
