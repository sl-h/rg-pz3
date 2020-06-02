using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;
using WpfApp1.Model;

namespace IveGrid3D
{
    public class ObjectWrapper : IPosition
    {
        public PowerEntity Entity;
        public GeometryModel3D Model { get; }
        /// <summary>
        /// Position in scene
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Position in scene
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// Position in scene
        /// </summary>
        public double Z { get; }

        public bool IsSelected { get; set; }
        public int ConnectionCount;
        public ObjectWrapper(GeometryModel3D model, PowerEntity entity)
        {
            Model = model;
            Entity = entity;
            X = model.Bounds.Location.X;
            Y = model.Bounds.Location.Y;
            Z = model.Bounds.Location.Z;
            if (Model != null)
            {
            }
        }

        void OnHoverOverNode(object sender, RoutedEventArgs e)
        {
            //MainWindow.tool.Content = ToolTipHelper.Serialize(new List<PowerEntity>() { Entity });

        }
    }
}