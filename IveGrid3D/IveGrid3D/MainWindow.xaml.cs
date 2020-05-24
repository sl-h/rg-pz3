using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace IveGrid3D
{
    public partial class MainWindow
    {
        private const string ImagePath = @"D:\fax\rg_pz3\IveGrid3D\IveGrid3D\Assets\Images\map.jpg";
        private readonly Model3DGroup mainModel3DGroup = new Model3DGroup();


        public MainWindow()
        {
            InitializeComponent();
            AddModelGroupToViewPort();
            Viewport.PanGesture = new MouseGesture(MouseAction.LeftClick);
            Viewport.RotateGesture = new MouseGesture(MouseAction.RightClick);
            DrawMapSurface();

        }

        private void AddModelGroupToViewPort()
        {
            var modelVisual = new ModelVisual3D { Content = mainModel3DGroup };
            Viewport.Children.Add(modelVisual);
        }

        private void DrawMapSurface()
        {
            var builder = new MeshBuilder();
            builder.AddCube(BoxFaces.Top);
            var cube = builder.ToMesh();

            var mat = new DiffuseMaterial { Brush = new ImageBrush(new BitmapImage(new Uri(ImagePath))) };

            var surfaceModel = new GeometryModel3D(cube, mat)
            {
                BackMaterial = mat,
                Transform = new ScaleTransform3D(-10, 10, 10)
            };

            mainModel3DGroup.Children.Add(surfaceModel);
        }
    }
}
