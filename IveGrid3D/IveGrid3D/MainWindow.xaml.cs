using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Xml;
using HelixToolkit.Wpf;
using WpfApp1.Model;

namespace IveGrid3D
{
    // Todo: +Calculate delta coefficient (min max coordinate to min max positions in 3d scene)
    // Todo: +Coordinate to position 
    // Todo: +Create and place cubes 

    public class ObjectWrapper
    {
        public GeometryModel3D Model { get; }

        public ObjectWrapper(GeometryModel3D model)
        {
            this.Model = model;
        }
    }


    public partial class MainWindow
    {
        private const double MapScale = 10;
        private const string ImagePath = @"D:\fax\rg_pz3\IveGrid3D\IveGrid3D\Assets\Images\map.jpg";
        private readonly Model3DGroup mainModel3DGroup = new Model3DGroup();
        private PositionMapper mapper;

        public MainWindow()
        {
            InitializeComponent();
            AddModelGroupToViewPort();
            Viewport.PanGesture = new MouseGesture(MouseAction.LeftClick);
            Viewport.RotateGesture = new MouseGesture(MouseAction.RightClick);
            InitializePositionMapper();
            DrawMapSurface();
            LoadData();

            //Viewport.Camera.LookDirection = new Vector3D(0, 0.00000000001, -1);

        }

        private void ParsePowerEntity(XmlNode node, PowerEntity entity)
        {
            entity.Id = long.Parse(node.SelectSingleNode("Id")?.InnerText ?? string.Empty);
            entity.Name = node.SelectSingleNode("Name")?.InnerText;
            entity.X = double.Parse(node.SelectSingleNode("X")?.InnerText ?? string.Empty);
            entity.Y = double.Parse(node.SelectSingleNode("Y")?.InnerText ?? string.Empty);
        }

        private ObjectWrapper Create3DObject(PowerEntity entity, System.Windows.Media.Color color, double scale)
        {
            Utility.ToLatLon(entity.X, entity.Y, 34, out var x, out var y);
            if (mapper.IsInRange(x, y)) return null;

            var position = mapper.Convert(x, y);


            var builder = new MeshBuilder();
            builder.AddCube();
            var cube = builder.ToMesh();

            var mat = new DiffuseMaterial { Brush = new SolidColorBrush(color) };

            var surfaceModel = new GeometryModel3D(cube, mat)
            {
                BackMaterial = mat,
                Transform = new Transform3DGroup()
                {
                    Children = new Transform3DCollection()
                    {
                         new ScaleTransform3D(scale, scale, scale),
                         new TranslateTransform3D(position.X,  position.Y, 0)
                    }

                }
            };

            mainModel3DGroup.Children.Add(surfaceModel);

            var newObject = new ObjectWrapper(surfaceModel);
            return newObject;
        }


        void LoadData()
        {
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load("Geographic.xml");
            if (xmlDoc.DocumentElement == null) return;

            var nodeListSubstation = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");
            var nodeListSwitch = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            var nodeListLines = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");
            var nodeListNode = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");


            foreach (XmlNode node in nodeListSubstation)
            {
                var entity = new SubstationEntity();
                ParsePowerEntity(node, entity);
                var spot = Create3DObject(entity, System.Windows.Media.Color.FromRgb(0, 255, 0), .1);
            }

            foreach (XmlNode node in nodeListNode)
            {
                var entity = new NodeEntity();
                ParsePowerEntity(node, entity);

                var spot = Create3DObject(entity, System.Windows.Media.Color.FromRgb(255, 255, 0), .1);
            }

            foreach (XmlNode node in nodeListSwitch)
            {
                var entity = new SwitchEntity();
                ParsePowerEntity(node, entity);
                var spot = Create3DObject(entity, System.Windows.Media.Color.FromRgb(0, 0, 255), .1);
            }

            //foreach (XmlNode item in nodeListLines)
            //{
            //    var line = new LineEntity();
            //    line.Id = long.Parse(item.SelectSingleNode("Id").InnerText);
            //    line.Name = item.SelectSingleNode("Name").InnerText;
            //    line.IsUnderground = bool.Parse(item.SelectSingleNode("IsUnderground").InnerText);
            //    line.LineType = item.SelectSingleNode("LineType").InnerText;
            //    line.R = float.Parse(item.SelectSingleNode("R").InnerText);
            //    line.FirstEnd = long.Parse(item.SelectSingleNode("FirstEnd").InnerText);
            //    line.SecondEnd = long.Parse(item.SelectSingleNode("SecondEnd").InnerText);
            //    line.ConductorMaterial = item.SelectSingleNode("ConductorMaterial").InnerText;
            //    line.ThermalConstantHeat = long.Parse(item.SelectSingleNode("ThermalConstantHeat").InnerText);
            //    lines.Add(line.Id, line);
            //}


            // foreach (var path in iterator.FindPaths(entities, spots, lines.Values.ToList()))
            {
                //#region Sa prekalapanjem
                //// Ovde je sa preklapanjem

                ////Polyline s = new Polyline();
                ////s.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                ////s.StrokeThickness = 0.5;

                ////foreach (var pat in paths.spots)
                ////{
                ////    s.Points.Add(new System.Windows.Point(pat.X, pat.Y));
                ////}
                ////canvas.Children.Add(s); 
                //#endregion
                //entityLineLines[path.lineEntityId] = new List<LineSegmentContainer>();
                //for (int i = 0; i < path.spots.Count - 1; i++)
                //{

                //    if (path.spots[i].IsOccupied == false || path.spots[i + 1].IsOccupied == false)  // overlap check
                //    {
                //        Polyline s = new Polyline();

                //        var p1 = new System.Windows.Point(path.spots[i].X, path.spots[i].Y);
                //        var p2 = new System.Windows.Point(path.spots[i + 1].X, path.spots[i + 1].Y);
                //        s.Points.Add(p1);
                //        s.Points.Add(p2);
                //        path.spots[i].AssignLinePart(path.lineEntityId);

                //        entityLineLines[path.lineEntityId].Add(new LineSegmentContainer() { line = s, p1 = path.spots[i], p2 = path.spots[i + 1] });
                //        if (path.spots[i].IsOccupied && path.spots[i].Entities.Count == 0)
                //        {
                //            path.spots[i].AssignCross();
                //        }

                //        path.spots[i].IsOccupied = true;
                //        s.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                //        s.StrokeThickness = 0.5;
                //        canvas.Children.Add(s);
                //        s.MouseRightButtonDown += path.spots[i].RightClickOnThis;
                //        s.MouseEnter += OnHoverOverLine;
                //    }
                //    else
                //    {
                //        bool found = false;
                //        foreach (var item in entityLineLines.Values)
                //        {
                //            if (found)
                //                break;
                //            foreach (var l in item)
                //            {
                //                if (path.spots[i] == l.p1 && path.spots[i + 1] == l.p2)
                //                {
                //                    found = true;
                //                    entityLineLines[path.lineEntityId].Add(l);
                //                    break;
                //                }
                //            }
                //        }
                //    }
                //}
            }
        }

        private void InitializePositionMapper()
        {
            mapper = new PositionMapper(MapScale, 0, 45.2325, 19.793909, 45.277031, 19.894459);

            // Tests
            //var testPoint = mapper.Convert(45.2547655, 19.844184);
            //var testPoint = mapper.Convert(45.277031, 19.894459);
            //var testPoint = mapper.Convert(45.2325, 19.793909);
        }
        private void AddModelGroupToViewPort()
        {
            var modelVisual = new ModelVisual3D { Content = mainModel3DGroup };
            Viewport.Children.Add(modelVisual);
        }

        private void DrawMapSurface()
        {
            var builder = new MeshBuilder();
            builder.AddCube(BoxFaces.Bottom);
            var cube = builder.ToMesh();

            var mat = new DiffuseMaterial { Brush = new ImageBrush(new BitmapImage(new Uri(ImagePath))) };

            var surfaceModel = new GeometryModel3D(cube, mat)
            {
                BackMaterial = mat,
                Transform = new Transform3DGroup()
                {
                    Children = new Transform3DCollection()
                    {
                        new ScaleTransform3D(MapScale, MapScale, 0),
                        new TranslateTransform3D(MapScale/2, -MapScale/2, 0),
                        new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0,0,1),-180 ))
                    }
                }
            };

            mainModel3DGroup.Children.Add(surfaceModel);
        }
    }
}
