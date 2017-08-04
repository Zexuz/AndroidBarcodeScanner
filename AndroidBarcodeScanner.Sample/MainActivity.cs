using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android;
using Android.Support.V4.App;
using Android.Content.PM;
using Android.Content;
using AndroidBarcodeScanner.Barcode;

namespace AndroidBarcodeScanner.Sample
{
    [Activity(Label = "AndroidBarcodeScanner.Sample", MainLauncher = true, Icon = "@drawable/icon")]
     public class MainActivity : FragmentActivity
    {
        Button scanbtn;
        Button fragmentbtn;
        public static int REQUEST_CODE = 100;
        public static int PERMISSION_REQUEST = 200;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);
            scanbtn = FindViewById<Button>(Resource.Id.scanbtn);
            fragmentbtn= FindViewById<Button>(Resource.Id.fragmentbtn);

            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.Camera }, PERMISSION_REQUEST);
            }

            scanbtn.Click += StartScanActivity;
            fragmentbtn.Click += StartScanFragment;
        }

        private void StartScanActivity(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(BarcodeSannerActivity));
            StartActivityForResult(intent, REQUEST_CODE);
        }
        
        public void StartScanFragment(object sender, EventArgs e)
        {
            FragmentManager
                .BeginTransaction()
                .Replace(Resource.Id.frameLayout1, new BarcodeSannerFragment())
                .Commit();
        }
      
    }
}

