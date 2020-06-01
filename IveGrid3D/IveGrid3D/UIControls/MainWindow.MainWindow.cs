using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using WpfApp1.Model;

namespace IveGrid3D
{
    public partial class MainWindow
    {
        private GeometryModel3D hitGeometry;

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
        void MainViewport_MouseDown(object sender, MouseButtonEventArgs e)
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

                DiffuseMaterial darkSide = new DiffuseMaterial(new SolidColorBrush(Colors.Red));
                bool gasit = false;
                foreach (var obj in instantiatedObject)
                {
                    if (obj.Model != rayResult.ModelHit) continue;
                    if (obj.IsSelected) continue;
                    gasit = true;
                    tooltip.Content = ToolTipHelper.Serialize(new List<PowerEntity>() { obj.Entity });
                    tooltip.IsOpen = true;
                    hitGeometry = (GeometryModel3D)rayResult.ModelHit;
                    ScheduleColorReset(obj);
                    hitGeometry.Material = darkSide;
                }
                foreach (var obj in lines.Values)
                {
                    if (obj.Model != rayResult.ModelHit) continue;
                    if (obj.IsSelected || obj.FirstEnd.IsSelected || obj.SecondEnd.IsSelected) continue;
                    gasit = true;
                    tooltip.IsOpen = true;

                    tooltip.Content = $"Type: Line Entity\nId: {obj.Entity.Id}\nName: {obj.Entity.Name}\nIsUnderground: {obj.Entity.IsUnderground}";
                    hitGeometry = (GeometryModel3D)rayResult.ModelHit;
                    ScheduleColorReset(obj);
                    ScheduleColorReset(obj.FirstEnd);
                    ScheduleColorReset(obj.SecondEnd);
                    hitGeometry.Material = darkSide;
                    obj.FirstEnd.Model.Material = darkSide;
                    obj.SecondEnd.Model.Material = darkSide;
                }
                if (!gasit)
                {
                    hitGeometry = null;
                }
            }

            return HitTestResultBehavior.Stop;
        }

        private async void ScheduleColorReset(IPosition position)
        {
            position.IsSelected = true;
            var previousMaterial = position.Model.Material;
            await DelayThenDoSomeWork();
            position.Model.Material = previousMaterial;
            position.IsSelected = false;
            tooltip.IsOpen = false;

        }

        private async Task DelayThenDoSomeWork()
        {
            await Task.Delay(2000);
        }
    }
}