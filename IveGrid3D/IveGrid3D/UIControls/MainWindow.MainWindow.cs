using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace IveGrid3D
{
    public partial class MainWindow
    {
        private readonly int zoomMax = 7;

        private Point start;
        private Point diffOffset;
        private int zoomCurrent = 1;


        private void OnLeftMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            Viewport.ReleaseMouseCapture();
        }

        private void OnLeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            Viewport.CaptureMouse();
            start = e.GetPosition(this);
            //diffOffset.X = TranslateContent.OffsetX;
            //diffOffset.Y = TranslateContent.OffsetY;
        }

        private void Pan(object sender, MouseEventArgs e)
        {
            if (Viewport.IsMouseCaptured)
            {
                var end = e.GetPosition(this);
                var offsetX = end.X - start.X;
                var offsetY = end.Y - start.Y;
                var w = this.Width;
                var h = this.Height;
                var translateX = (offsetX * 100) / w;
                var translateY = -(offsetY * 100) / h;
                var position = new Matrix3D();
                //position.OffsetX = diffOffset.X + (translateX / (100 * ScaleContent.ScaleX));
                //position.OffsetY = diffOffset.Y + (translateY / (100 * ScaleContent.ScaleX));
                //Viewport.Camera.Transform.SetCurrentValue();
                // TranslateContent.OffsetX = diffOffset.X + (translateX / (100 * ScaleContent.ScaleX));
                //TranslateContent.OffsetY = diffOffset.Y + (translateY / (100 * ScaleContent.ScaleX));
            }
        }
        private void MouseWheal(object sender, MouseWheelEventArgs e)
        {
            double scaleX;
            double scaleY;
            //if (e.Delta > 0 && zoomCurrent < zoomMax)
            //{
            //    scaleX = ScaleContent.ScaleX + 0.1;
            //    scaleY = ScaleContent.ScaleY + 0.1;
            //    zoomCurrent++;
            //    ScaleContent.ScaleX = scaleX;
            //    ScaleContent.ScaleY = scaleY;
            //}
            //else if (e.Delta <= 0 && zoomCurrent > -zoomMax)
            //{
            //    scaleX = ScaleContent.ScaleX - 0.1;
            //    scaleY = ScaleContent.ScaleY - 0.1;
            //    zoomCurrent--;
            //    ScaleContent.ScaleX = scaleX;
            //    ScaleContent.ScaleY = scaleY;
            //}
        }
    }
}