using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android;
using Android.Support.V4.App;
using Android.Content.PM;
using Android.Content;
using AndroidBarcodeScanner.Barcode;

namespace AndroidBarcodeScanner.Sample
{
    [Activity(Label = "AndroidBarcodeScanner.Sample", MainLauncher = true, Icon = "@drawable/icon")]
     public class MainActivity : Activity, View.IOnClickListener
    {
        Button scanbtn;
        TextView result;
        public static int REQUEST_CODE = 100;
        public static int PERMISSION_REQUEST = 200;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);
            scanbtn = FindViewById<Button>(Resource.Id.scanbtn);
            result = FindViewById<TextView>(Resource.Id.result);

            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.Camera }, PERMISSION_REQUEST);
            }

            scanbtn.SetOnClickListener(this);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == REQUEST_CODE && resultCode == Result.Ok)
            {
                if (data != null)
                {
                    var barcode = (Android.Gms.Vision.Barcodes.Barcode) data.GetParcelableExtra("barcode");
                    result.Post(() => {
                        result.Text = barcode.DisplayValue;
                    });
                }
            }
        }


        public void OnClick(View v)
        {
            Intent intent = new Intent(this, typeof(BarcodeSannerActivity));
            StartActivityForResult(intent, REQUEST_CODE);
        }
    }
}

