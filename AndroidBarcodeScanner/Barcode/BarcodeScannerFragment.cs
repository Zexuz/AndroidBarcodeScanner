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
    public class BarcodeSannerFragment : Fragment
    {
        private const string Tag = "BarcodeTracker";
      
        private View view;

        public Scanner Scanner { get; private set; }

        public override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.ScanLayout, container, false);
            return view;
        }

        public override void OnViewStateRestored(Bundle savedInstanceState)
        {
            base.OnViewStateRestored(savedInstanceState);
            
            var preview = view.FindViewById<CameraSourcePreview>(Resource.Id.preview);
            var graphicOverlay = view.FindViewById<GraphicOverlay>(Resource.Id.faceOverlay);

            if (Scanner == null)
            {
                Scanner = new Scanner(preview,graphicOverlay,Activity.ApplicationContext);
            }
            Scanner.InitCameraSource();
        }

        public override void OnResume()
        {
            base.OnResume();
           
            Scanner.OnResume();
        }
        

        public override void OnPause()
        {
            base.OnPause();
            Scanner.OnPause();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Scanner.OnDestroy();
        }
    }
}