using System.Collections.Generic;
using Android.Content;
using Android.Gms.Vision;
using Android.Graphics;
using Android.Util;
using Android.Views;

namespace AndroidBarcodeScanner.Barcode.UI
{
    public class GraphicOverlay : View
    {
        private readonly object _lock = new object();
        private int _previewWidth;
        private float _widthScaleFactor = 1.0f;
        private int _previewHeight;
        private float _heightScaleFactor = 1.0f;
        private CameraFacing _facing = CameraFacing.Back;
        private readonly HashSet<Graphic> _graphics = new HashSet<Graphic>();

        /**
     * Base class for a custom graphics object to be rendered within the graphic overlay.  Subclass
     * this and implement the {@link Graphic#draw(Canvas)} method to define the
     * graphics element.  Add instances to the overlay using {@link GraphicOverlay#add(Graphic)}.
     */
        public abstract class Graphic
        {
            private readonly GraphicOverlay _overlay;

            protected Graphic(GraphicOverlay overlay)
            {
                _overlay = overlay;
            }

            /**
         * Draw the graphic on the supplied canvas.  Drawing should use the following methods to
         * convert to view coordinates for the graphics that are drawn:
         * <ol>
         * <li>{@link Graphic#scaleX(float)} and {@link Graphic#scaleY(float)} adjust the size of
         * the supplied value from the preview scale to the view scale.</li>
         * <li>{@link Graphic#translateX(float)} and {@link Graphic#translateY(float)} adjust the
         * coordinate from the preview's coordinate system to the view coordinate system.</li>
         * </ol>
         *
         * @param canvas drawing canvas
         */
            public abstract void Draw(Canvas canvas);

            /**
         * Adjusts a horizontal value of the supplied value from the preview scale to the view
         * scale.
         */
            public float ScaleX(float horizontal)
            {
                return horizontal * _overlay._widthScaleFactor;
            }

            /**
         * Adjusts a vertical value of the supplied value from the preview scale to the view scale.
         */
            public float ScaleY(float vertical)
            {
                return vertical * _overlay._heightScaleFactor;
            }

            /**
         * Adjusts the x coordinate from the preview's coordinate system to the view coordinate
         * system.
         */
            public float TranslateX(float x)
            {
                if (_overlay._facing == CameraFacing.Front)
                {
                    return _overlay.Width - ScaleX(x);
                }
                
                return ScaleX(x);
            }

            /**
         * Adjusts the y coordinate from the preview's coordinate system to the view coordinate
         * system.
         */
            public float TranslateY(float y)
            {
                return ScaleY(y);
            }

            public void PostInvalidate()
            {
                _overlay.PostInvalidate();
            }
        }

        public GraphicOverlay(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        /**
     * Removes all graphics from the overlay.
     */
        public void Clear()
        {
            lock (_lock)
            {
                _graphics.Clear();
            }
            PostInvalidate();
        }

        /**
     * Adds a graphic to the overlay.
     */
        public void Add(Graphic graphic)
        {
            lock (_lock)
            {
                _graphics.Add(graphic);
            }
            PostInvalidate();
        }

        /**
     * Removes a graphic from the overlay.
     */
        public void Remove(Graphic graphic)
        {
            lock (_lock)
            {
                _graphics.Remove(graphic);
            }
            PostInvalidate();
        }

        /**
     * Sets the camera attributes for size and facing direction, which informs how to transform
     * image coordinates later.
     */
        public void SetCameraInfo(int previewWidth, int previewHeight, CameraFacing facing)
        {
            lock (_lock)
            {
                _previewWidth = previewWidth;
                _previewHeight = previewHeight;
                _facing = facing;
            }
            PostInvalidate();
        }

        /**
     * Draws the overlay with its associated graphic objects.
     */

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            lock (_lock)
            {
                if ((_previewWidth != 0) && (_previewHeight != 0))
                {
                    _widthScaleFactor = (float) canvas.Width / (float) _previewWidth;
                    _heightScaleFactor = (float) canvas.Height / (float) _previewHeight;
                }

                foreach (var graphic in _graphics)
                {
                    graphic.Draw(canvas);
                }
            }
        }
    }
}