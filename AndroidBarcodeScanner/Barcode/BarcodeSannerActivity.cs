using System;
using Android;
using Android.App;
using Android.Gms.Vision;
using Android.Gms.Vision.Barcodes;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidBarcodeScanner.Barcode.UI;

namespace AndroidBarcodeScanner.Barcode
{
    [Activity(Label = "BarcodeScannerActivity")]
    public class BarcodeSannerActivity : Activity
    {
        private const string Tag = "BarcodeTracker";

        private Scanner _scanner;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.ScanLayout);

            var preview = FindViewById<CameraSourcePreview>(Resource.Id.preview);
            var graphicOverlay = FindViewById<GraphicOverlay>(Resource.Id.faceOverlay);

            _scanner = new Scanner(preview, graphicOverlay, ApplicationContext);
            _scanner.InitCameraSource();
        }

        protected override void OnResume()
        {
            base.OnResume();
            _scanner.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();
            _scanner.OnPause();
        }

        protected override void OnDestroy()
        {
            _scanner.OnDestroy();
            base.OnDestroy();
        }
    }
}