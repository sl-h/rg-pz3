using System.Windows.Media.Media3D;
using WpfApp1.Model;

namespace IveGrid3D
{
    public class ObjectWrapper
    {
        public PowerEntity Entity;
        public GeometryModel3D Model { get; }
        /// <summary>
        /// Position in scene
        /// </summary>
        public readonly double X;

        /// <summary>
        /// Position in scene
        /// </summary>
        public readonly double Y;

        /// <summary>
        /// Position in scene
        /// </summary>
        public double Z;

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
    }
}