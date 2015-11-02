# UsbCamera Class
This is the code block for using a USB camera with Windows 10 IoT Core.

## Methods
The UsbCamera class has these methods:
- **InitializeCameraAsync:** Initializes the first detected camera device asynchronously.
- **StartCameraPreview:** Begins live USB camera feed asynchronously.
- **StopCameraPreview:** Ends live USB camera feed asynchronously.
- **CapturePhoto:** Captures photo from camera feed and stores it in local storage asynchronously. Returns image file as a StorageFile. File is stored in a temporary folder and could be deleted by the system at any time.
- **Dispose:** Performs tasks associated with freeing, releasing, or resetting unmanaged resources.

## Usage
To use this block in your project, you need to perform the following steps:

1. Navigate to your git project folder using Command Prompt and run `git submodule add https://github.com/ms-iot/usb-camera`
2. Next, run `git submodule update`
3. Open your project solution on Visual Studio and right click on Solution -> Add -> Existing Project. Select usb-camera -> UsbCamera -> UsbCamera.csproj.
4. Once usb-camera is added to the solution explorer, right click on References on your project -> Add Reference -> Projects -> Solution. Check UsbCamera and select OK.
5. You should now be able to use UsbCamera objects in your project.

Note: Everytime you clone your project after it's initial creation, you must run the following commands in the project's root folder: 
- `git submodule init`
- `git submodule update`
