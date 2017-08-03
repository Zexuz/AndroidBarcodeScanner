using System;
using Android;
using Android.App;
using Android.Gms.Vision;
using Android.Gms.Vision.Barcodes;
using Android.OS;
using Android.Runtime;
using AndroidBarcodeScanner.Barcode.UI;

namespace AndroidBarcodeScanner.Barcode
{
    [Activity(Label = "BarcodeScannerActivity")]
    public class BarcodeSannerActivity : Activity
    {
        private const string Tag = "BarcodeTracker";

        private CameraSource _cameraSource;
        private CameraSourcePreview _preview;
        private GraphicOverlay _graphicOverlay;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.ScanLayout);

            _preview = FindViewById<CameraSourcePreview>(Resource.Id.preview);
            _graphicOverlay = FindViewById<GraphicOverlay>(Resource.Id.faceOverlay);

            var detector = new BarcodeDetector.Builder(Application.Context)
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

            _cameraSource = new CameraSource.Builder(Application.Context, detector)
                .SetRequestedPreviewSize(640, 480)
                .SetFacing(CameraFacing.Back)
                .SetRequestedFps(24.0f)
                .SetAutoFocusEnabled(true)
                .Build();
        }

        protected override void OnResume()
        {
            base.OnResume();

            StartCameraSource();
        }

        protected override void OnPause()
        {
            base.OnPause();

            _preview.Stop();
        }

        protected override void OnDestroy()
        {
            _cameraSource.Release();

            base.OnDestroy();
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

        //==============================================================================================
        // Graphic Face Tracker
        //==============================================================================================

        /**
        * Factory for creating a face tracker to be associated with a new face.  The multiprocessor
        * uses this factory to create face trackers as needed -- one for each individual.
        */
        private class GraphicBarcodeTrackerFactory : Java.Lang.Object, MultiProcessor.IFactory
        {
            public GraphicBarcodeTrackerFactory(GraphicOverlay overlay) : base()
            {
                Overlay = overlay;
            }

            private GraphicOverlay Overlay { get; set; }

            public Android.Gms.Vision.Tracker Create(Java.Lang.Object item)
            {
                return new GraphicBarcodeTracker(Overlay);
            }
        }

        /**
        * Face tracker for each detected individual. This maintains a face graphic within the app's
        * associated face overlay.
        */
        private class GraphicBarcodeTracker : Tracker
        {
            private readonly GraphicOverlay _overlay;
            private readonly BarcodeGraphic _barcodeGraphic;

            public GraphicBarcodeTracker(GraphicOverlay overlay)
            {
                _overlay = overlay;
                _barcodeGraphic = new BarcodeGraphic(overlay);
            }

            /**
            * Start tracking the detected face instance within the face overlay.
            */
            public override void OnNewItem(int idValue, Java.Lang.Object item)
            {
                _barcodeGraphic.Id = idValue;
            }

            /**
            * Update the position/characteristics of the face within the overlay.
            */
            public override void OnUpdate(Detector.Detections detections, Java.Lang.Object item)
            {
                _overlay.Add(_barcodeGraphic);
                _barcodeGraphic.UpdateBarcode(item.JavaCast<Android.Gms.Vision.Barcodes.Barcode>());
            }

            /**
            * Hide the graphic when the corresponding face was not detected.  This can happen for
            * intermediate frames temporarily (e.g., if the face was momentarily blocked from
            * view).
            */
            public override void OnMissing(Detector.Detections detections)
            {
                _overlay.Remove(_barcodeGraphic);
            }

            /**
            * Called when the face is assumed to be gone for good. Remove the graphic annotation from
            * the overlay.
            */
            public override void OnDone()
            {
                _overlay.Remove(_barcodeGraphic);
            }
        }
    }
}