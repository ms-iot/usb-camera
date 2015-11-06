using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;

namespace Microsoft.Maker.Devices.Media.UsbCamera
{
    public class UsbCamera
    {
        /// <summary>
        /// Media Capture object for the USB camera
        /// </summary>
        private MediaCapture mediaCapture;

        /// <summary>
        /// Control Flag
        /// </summary>
        private bool isInitialized = false;

        /// <summary>
        /// Gets the MediaCapture object for the USB camera 
        /// </summary>
        public MediaCapture MediaCaptureInstance
        {
            get { return mediaCapture; }
        }

        /// <summary>
        /// Asynchronously initializes webcam feed
        /// </summary>
        /// <returns>
        /// Task object: True if camera is successfully initialized; false otherwise.
        /// </returns>
        public async Task<bool> InitializeAsync()
        {
            if (mediaCapture == null)
            {
                // Attempt to get attached webcam
                var cameraDevice = await FindCameraDevice();

                if (cameraDevice == null)
                {
                    // No camera found, report the error and break out of initialization
                    Debug.WriteLine("UsbCamera: No camera found!");
                    isInitialized = false;
                    return false;
                }

                // Creates MediaCapture initialization settings with foudnd webcam device
                var settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id };

                mediaCapture = new MediaCapture();
                try
                {
                    await mediaCapture.InitializeAsync(settings);
                    isInitialized = true;
                    return true;
                }
                catch (UnauthorizedAccessException ex)
                {
                    Debug.WriteLine("UsbCamera: UnauthorizedAccessException: " + ex.ToString() + "Ensure webcam capability is added in the manifest.");
                    throw;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("UsbCamera: Exception when initializing MediaCapture:" + ex.ToString());
                    throw;
                }
            }
            return false;
        }

        /// <summary>
        /// Asynchronously begins live webcam feed
        /// </summary>
        /// <returns>
        /// Task object.
        /// </returns>
        public async Task StartCameraPreview()
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
        /// Asynchronously captures photo from camera feed and stores it in local storage. Returns image file as a StorageFile.
        /// File is stored in a temporary folder and could be deleted by the system at any time.
        /// </summary>
        /// <returns>
        /// Task object: Storage file with captured image.
        /// </returns>
        public async Task<StorageFile> CapturePhoto()
        {
            StorageFile file;

            // Create storage file in local app storage

            string fileName = GenerateNewFileName() + ".jpg";
            CreationCollisionOption collisionOption = CreationCollisionOption.GenerateUniqueName;

            file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(fileName, collisionOption);

            // Captures and stores new Jpeg image file
            await mediaCapture.CapturePhotoToStorageFileAsync(ImageEncodingProperties.CreateJpeg(), file);

            // Return image file
            return file;
        }

        /// <summary>
        /// Returns true if webcam has been successfully initialized. Otherwise, returns false.
        /// </summary>
        /// <returns>
        /// Returns true if camera is initialized; false otherwise.
        /// </returns>
        public bool IsInitialized()
        {
            return isInitialized;
        }

        /// <summary>
        /// Asynchronously ends live webcam feed
        /// </summary>
        /// <returns>
        /// Task object.
        /// </returns>
        public async Task StopCameraPreview()
        {
            try
            {
                await mediaCapture.StopPreviewAsync();
            }
            catch
            {
                Debug.WriteLine("UsbCamera: Failed to stop camera preview stream");
                throw;
            }
        }

        /// <summary>
        /// Performs tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            mediaCapture?.Dispose();
            isInitialized = false;
        }

        /// <summary>
        /// Asynchronously looks for and returns first camera device found.
        /// If no device is found, return null
        /// </summary>
        /// <returns>
        /// Task object: Device information of the first detected camera device.
        /// </returns>
        private static async Task<DeviceInformation> FindCameraDevice()
        {
            // Get available devices for capturing pictures
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            return allVideoDevices.Count > 0 ? allVideoDevices[0] : null;
        }

        /// <summary>
        /// Generates unique file name based on current time and date. Returns value as string.
        /// </summary>
        /// <param name = "prefix">
        /// Prefix for image name. Default value = "IMG".
        /// </param>
        /// <returns>
        /// Unique file name string.
        /// </returns>
        private string GenerateNewFileName(string prefix = "IMG")
        {
            return prefix + "_" + DateTime.UtcNow.ToString("yyyy-MMM-dd_HH-mm-ss");
        }
    }
}

