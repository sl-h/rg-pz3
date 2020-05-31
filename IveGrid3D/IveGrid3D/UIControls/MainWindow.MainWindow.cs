using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace IveGrid3D
{
    public partial class MainWindow
    {
        private GeometryModel3D hitGeometry;
        private readonly DiffuseMaterial blue = new DiffuseMaterial()
        {
            Color = Colors.Blue
        };


        private void OnLeftMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            Viewport.ReleaseMouseCapture();
        }

        private void OnLeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point mouseposition = e.GetPosition(Viewport);
            Point3D testpoint3D = new Point3D(mouseposition.X, mouseposition.Y, 0);
            Vector3D testdirection = new Vector3D(mouseposition.X, mouseposition.Y, 10);

            PointHitTestParameters pointparams =
                new PointHitTestParameters(mouseposition);
            RayHitTestParameters rayparams =
                new RayHitTestParameters(testpoint3D, testdirection);

            //test for a result in the Viewport3D
            hitGeometry = null;
            VisualTreeHelper.HitTest(Viewport, null, HTResult, pointparams);
        }

        private void Pan(object sender, MouseEventArgs e)
        {


        }

        private void MouseWheal(object sender, MouseWheelEventArgs e)
        {

        }
        void mainViewport_MouseDown(object sender, MouseButtonEventArgs e)
        {

            var mousePosition = e.GetPosition(Viewport);
            var testPoint3D = new Point3D(mousePosition.X, mousePosition.Y, 0);
            var testDirection = new Vector3D(mousePosition.X, mousePosition.Y, 10);

            var pointParams = new PointHitTestParameters(mousePosition);
            var rayParams = new RayHitTestParameters(testPoint3D, testDirection);

            //test for a result in the Viewport3D     
            hitGeometry = null;
            VisualTreeHelper.HitTest(Viewport, null, HTResult, pointParams);
        }


        private HitTestResultBehavior HTResult(HitTestResult rawresult)
        {
            RayHitTestResult rayResult = rawresult as RayHitTestResult;

            if (rayResult != null)
            {

                DiffuseMaterial darkSide =
                    new DiffuseMaterial(new SolidColorBrush(
                        System.Windows.Media.Colors.Red));
                bool gasit = false;
                for (int i = 0; i < instantiatedObject.Count; i++)
                {
                    if (instantiatedObject[i].Model == rayResult.ModelHit)
                    {
                        hitGeometry = (GeometryModel3D)rayResult.ModelHit;
                        gasit = true;
                        hitGeometry.Material = darkSide;
                    }
                }
                if (!gasit)
                {
                    hitGeometry = null;
                }
            }

            return HitTestResultBehavior.Stop;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}