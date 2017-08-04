using System;
using Android.Content;
using Android.Gms.Vision;
using Android.Gms.Vision.Barcodes;
using Android.Runtime;
using AndroidBarcodeScanner.Barcode.UI;

namespace AndroidBarcodeScanner.Barcode
{
    public class Scanner
    {
        private const string Tag = "Scanner";
        private CameraSource _cameraSource;
        private CameraSourcePreview _preview;
        private GraphicOverlay _graphicOverlay;
        private readonly Context _applicationContext;

        public Scanner(CameraSourcePreview preview, GraphicOverlay graphicOverlay, Context applicationContext)
        {
            _preview = preview;
            _graphicOverlay = graphicOverlay;
            _applicationContext = applicationContext;
        }

        public void InitCameraSource()
        {
            var detector = new BarcodeDetector.Builder(_applicationContext)
                .Build();
            detector.SetProcessor(
                new MultiProcessor.Builder(new GraphicBarcodeTrackerFactory(_graphicOverlay)).Build());

            if (!detector.IsOperational)
            {
                // Note: The first time that an app using barcode API is installed on a device, GMS will
                // download a native library to the device in order to do detection.  Usually this
                // completes before the app is run for the first time.  But if that download has not yet
                // completed, then the above call will not detect any barcodes.
                //
                // IsOperational can be used to check if the required native library is currently
                // available.  The detector will automatically become operational once the library
                // download completes on device.
                Android.Util.Log.Warn(Tag, "Barcode detector dependencies are not yet available.");
            }

            _cameraSource = new CameraSource.Builder(_applicationContext, detector)
                .SetRequestedPreviewSize(640, 480)
                .SetFacing(CameraFacing.Back)
                .SetRequestedFps(15.0f)
                .SetAutoFocusEnabled(true)
                .Build();

            StartCameraSource();
        }

        public void OnResume()
        {
            StartCameraSource();
        }

        
        public void OnPause()
        {
            _preview.Stop();
        }

        public void OnDestroy()
        {
            _cameraSource.Release();
        }


        //==============================================================================================
        // Camera Source Preview
        //==============================================================================================

        /**
          * Starts or restarts the camera source, if it exists.  If the camera source doesn't exist yet
          * (e.g., because onResume was called before the camera source was created), this will be called
          * again when the camera source is created.
          */
        private void StartCameraSource()
        {
            try
            {
                _preview.Start(_cameraSource, _graphicOverlay);
            }
            catch (Exception e)
            {
                Android.Util.Log.Error(Tag, "Unable to start camera source.", e);
                _cameraSource.Release();
                _cameraSource = null;
            }
        }
    }
}