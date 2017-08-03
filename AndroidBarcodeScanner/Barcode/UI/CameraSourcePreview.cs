using System;
using Android.Content;
using Android.Gms.Vision;
using Android.Util;
using Android.Views;

namespace AndroidBarcodeScanner.Barcode.UI
{
    public sealed class CameraSourcePreview : ViewGroup
    {
        public const string Tag = "CameraSourcePreview";

        private readonly Context _context;
        private readonly SurfaceView _surfaceView;
        private bool _startRequested;
        private bool SurfaceAvailable { get; set; }
        private CameraSource _cameraSource;

        private GraphicOverlay _overlay;

        public CameraSourcePreview(Context context, IAttributeSet attrs) : base (context, attrs)
        {
            this._context = context;
            _startRequested = false;
            SurfaceAvailable = false;

            _surfaceView = new SurfaceView (context);
            _surfaceView.Holder.AddCallback (new SurfaceCallback (this));
            AddView (_surfaceView);
        }

        public void Start (CameraSource cameraSource)
        {
            if (cameraSource == null)
                Stop();

            _cameraSource = cameraSource;

            if (_cameraSource != null) {
                _startRequested = true;
                StartIfReady ();
            }
        }

        public void Start (CameraSource cameraSource, GraphicOverlay overlay)
        {
            _overlay = overlay;
            Start (cameraSource);
        }

        public void Stop ()
        {
            if (_cameraSource != null)
                _cameraSource.Stop ();
        }

        public void Release ()
        {
            if (_cameraSource != null) {
                _cameraSource.Release ();
                _cameraSource = null;
            }
        }

        private void StartIfReady ()
        {
            if (_startRequested && SurfaceAvailable) {
                _cameraSource.Start (_surfaceView.Holder);
                if (_overlay != null) {
                    var size = _cameraSource.PreviewSize;
                    var min = Math.Min(size.Width, size.Height);
                    var max = Math.Max(size.Width, size.Height);
                    if (IsPortraitMode ()) {
                        // Swap width and height sizes when in portrait, since it will be rotated by
                        // 90 degrees
                        _overlay.SetCameraInfo (min, max, _cameraSource.CameraFacing);
                    } else {
                        _overlay.SetCameraInfo (max, min, _cameraSource.CameraFacing);
                    }
                    _overlay.Clear();
                }
                _startRequested = false;
            }
        }

        private class SurfaceCallback : Java.Lang.Object, ISurfaceHolderCallback
        {
            public SurfaceCallback (CameraSourcePreview parent)
            {
                Parent = parent;
            }

            private CameraSourcePreview Parent { get; }

            public void SurfaceCreated (ISurfaceHolder surface)
            {
                Parent.SurfaceAvailable = true;
                try {
                    Parent.StartIfReady ();
                } catch (Exception ex) {
                    Android.Util.Log.Error (Tag, "Could not start camera source.", ex);
                }
            }

            public void SurfaceDestroyed (ISurfaceHolder surface)
            {
                Parent.SurfaceAvailable = false;
            }

            public void SurfaceChanged (ISurfaceHolder holder, Android.Graphics.Format format, int width, int height)
            {
            }
        }
            
        protected override void OnLayout (bool changed, int left, int top, int right, int bottom)
        {
            int width = 320;
            int height = 240;
            if (_cameraSource != null) {
                var size = _cameraSource.PreviewSize;
                if (size != null) {
                    width = size.Width;
                    height = size.Height;
                }
            }

            // Swap width and height sizes when in portrait, since it will be rotated 90 degrees
            if (IsPortraitMode ()) {
                int tmp = width;
                width = height;
                height = tmp;
            }

            var layoutWidth = right - left;
            var layoutHeight = bottom - top;

            // Computes height and width for potentially doing fit width.
            int childWidth = layoutWidth;
            int childHeight = (int)(((float) layoutWidth / (float) width) * height);

            // If height is too tall using fit width, does fit height instead.
            if (childHeight > layoutHeight) {
                childHeight = layoutHeight;
                childWidth = (int)(((float) layoutHeight / (float) height) * width);
            }

            for (int i = 0; i < ChildCount; ++i)
                GetChildAt (i).Layout (0, 0, childWidth, childHeight);

            try {
                StartIfReady ();
            } catch (Exception ex) {
                Android.Util.Log.Error (Tag, "Could not start camera source.", ex);
            }
        }

        bool IsPortraitMode ()
        {
            var orientation = _context.Resources.Configuration.Orientation;
            if (orientation == Android.Content.Res.Orientation.Landscape)
                return false;            
            if (orientation == Android.Content.Res.Orientation.Portrait)
                return true;

            Android.Util.Log.Debug (Tag, "isPortraitMode returning false by default");
            return false;
        }
    }
}