using Android.Gms.Vision;
using Android.Runtime;
using AndroidBarcodeScanner.Barcode.UI;

namespace AndroidBarcodeScanner.Barcode
{
    //==============================================================================================
    // Graphic Barcode Tracker
    //==============================================================================================

    /**
    * Factory for creating a barcode tracker to be associated with a new barcode.  The multiprocessor
    * uses this factory to create barcode trackers as needed -- one for each individual.
    */
    public class GraphicBarcodeTrackerFactory : Java.Lang.Object, MultiProcessor.IFactory
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

        /**
       * Barcode tracker for each detected individual. This maintains a barcode graphic within the app's
       * associated barcode overlay.
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
            * Start tracking the detected barcode instance within the barcode overlay.
            */
            public override void OnNewItem(int idValue, Java.Lang.Object item)
            {
                _barcodeGraphic.Id = idValue;
            }

            /**
            * Update the position/characteristics of the barcode within the overlay.
            */
            public override void OnUpdate(Detector.Detections detections, Java.Lang.Object item)
            {
                _overlay.Add(_barcodeGraphic);
                _barcodeGraphic.UpdateBarcode(item.JavaCast<Android.Gms.Vision.Barcodes.Barcode>());
            }

            /**
            * Hide the graphic when the corresponding barcode was not detected.  This can happen for
            * intermediate frames temporarily (e.g., if the barcode was momentarily blocked from
            * view).
            */
            public override void OnMissing(Detector.Detections detections)
            {
                _overlay.Remove(_barcodeGraphic);
            }

            /**
            * Called when the barcode is assumed to be gone for good. Remove the graphic annotation from
            * the overlay.
            */
            public override void OnDone()
            {
                _overlay.Remove(_barcodeGraphic);
            }
        }
    }
}