using System.Windows;
using HelixToolkit.Wpf;

namespace IveGrid3D
{
    public class PositionMapper
    {
        private readonly double xScale;
        private readonly double yScale;
        private readonly double xMin;
        private readonly double yMin;
        private readonly double xMax;
        private readonly double yMax;
        private readonly double scaleBasedOffset;
        public PositionMapper(double mapScale, double mapCenter, double xMin, double yMin, double xMax, double yMax)
        {
            this.xMin = xMin;
            this.yMin = yMin;
            this.xMax = xMax;
            this.yMax = yMax;
            scaleBasedOffset = mapCenter - mapScale / 2;

            xScale = (xMax - xMin) / mapScale * -1;
            yScale = (yMax - yMin) / mapScale;
        }

        public Point Convert(double x, double y)
        {
            double xScaled = (x - xMin) / xScale;
            double yScaled = (y - yMin) / yScale;
            return new Point(xScaled + scaleBasedOffset, yScaled + scaleBasedOffset);
        }

        public bool IsInRange(double lon, double lat)
        {
            return ((lon < xMax) & (lon > xMin) & (lat < yMax) & (lat > yMin));
        }

        public bool IsInSceneRange(double x, double y)
        {
            return ((x <= 5) & (x > -5) & (y < 5) & (y > -5));
        }
    }
}