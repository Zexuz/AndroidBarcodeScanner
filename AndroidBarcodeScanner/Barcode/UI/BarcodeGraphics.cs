using System;
using Android.Graphics;

namespace AndroidBarcodeScanner.Barcode.UI
{
     public class BarcodeGraphic : GraphicOverlay.Graphic 
    {
        const float IdTextSize = 40.0f;
        const float IdYOffset = 50.0f;
        const float IdXOffset = -50.0f;
        const float BoxStrokeWidth = 5.0f;

        readonly Color[] COLOR_CHOICES = {
            Color.Blue,
            Color.Cyan,
            Color.Green,
            Color.Magenta,
            Color.Red,
            Color.White,
            Color.Yellow
        };

        static int _currentColorIndex = 0;

        private readonly Paint _positionPaint;
        private readonly Paint _idPaint;
        private readonly Paint _boxPaint;

        private Android.Gms.Vision.Barcodes.Barcode _barcode;
        public int Id { get;set; }

        public BarcodeGraphic (GraphicOverlay overlay) : base (overlay)
        {            
            _currentColorIndex = (_currentColorIndex + 1) % COLOR_CHOICES.Length;
            var selectedColor = COLOR_CHOICES[_currentColorIndex];

            _positionPaint = new Paint();
            _positionPaint.Color = selectedColor;

            _idPaint = new Paint();
            _idPaint.Color = selectedColor;
            _idPaint.TextSize = IdTextSize;

            _boxPaint = new Paint();
            _boxPaint.Color = selectedColor;
            _boxPaint.SetStyle (Paint.Style.Stroke);
            _boxPaint.StrokeWidth = BoxStrokeWidth;
        }

        /**
         * Updates the face instance from the detection of the most recent frame.  Invalidates the
         * relevant portions of the overlay to trigger a redraw.
         */
        public void UpdateBarcode (Android.Gms.Vision.Barcodes.Barcode barcode)
        {
            Console.WriteLine ("Barcode Format: {0}", barcode.Format);
            Console.WriteLine ("Value Format: {0}", barcode.ValueFormat);

            _barcode = barcode;
            PostInvalidate ();
        }

        /**
         * Draws the face annotations for position on the supplied canvas.
         */
        public override void Draw (Canvas canvas) 
        {
            Android.Gms.Vision.Barcodes.Barcode barcode = _barcode;
            if (barcode == null)
                return;

            var rect = new Rect(barcode.BoundingBox);
            rect.Left = (int) TranslateX(rect.Left);
            rect.Top = (int) TranslateY(rect.Top);
            rect.Right = (int) TranslateX(rect.Right);
            rect.Bottom = (int) TranslateY(rect.Bottom);
            canvas.DrawRect(rect,_boxPaint);

            canvas.DrawText (barcode.DisplayValue,rect.Left + IdXOffset,rect.Bottom + IdYOffset, _idPaint);
        }
    }
}