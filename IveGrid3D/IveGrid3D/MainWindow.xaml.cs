using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Xml;
using HelixToolkit.Wpf;
using WpfApp1.Model;
using Point = WpfApp1.Model.Point;

namespace IveGrid3D
{
    public interface IPosition
    {
        double X { get; }
        double Y { get; }
        double Z { get; }
        bool IsSelected { get; set; }
        GeometryModel3D Model { get; }
    }

    public class LineContainer : IPosition
    {
        public LineContainer(ObjectWrapper firstEnd, ObjectWrapper secondEnd, LineEntity entity, GeometryModel3D model)
        {
            FirstEnd = firstEnd;
            SecondEnd = secondEnd;
            Entity = entity;
            Model = model;
        }

        public ObjectWrapper FirstEnd { get; }
        public ObjectWrapper SecondEnd { get; }
        public LineEntity Entity { get; }
        public GeometryModel3D Model { get; }

        public double X => Model.Bounds.Location.X;
        public double Y => Model.Bounds.Location.Y;
        public double Z => Model.Bounds.Location.Z;
        public bool IsSelected { get; set; }
    }


    public partial class MainWindow
    {
        private const double MapScale = 10;
        private const string ImagePath = @"D:\fax\rg_pz3\IveGrid3D\IveGrid3D\Assets\Images\map.jpg";
        private readonly Model3DGroup mainModel3DGroup = new Model3DGroup();
        private readonly List<ObjectWrapper> instantiatedObject = new List<ObjectWrapper>();
        private readonly Dictionary<long, LineContainer> lines = new Dictionary<long, LineContainer>();

        private PositionMapper mapper;
        public static ToolTip tooltip;


        void OnHoverOverNode(object sender, RoutedEventArgs e)
        {

        }

        public MainWindow()
        {
            tooltip = new ToolTip();
            InitializeComponent();
            AddModelGroupToViewPort();
            Viewport.PanGesture = new MouseGesture(MouseAction.MiddleClick);
            Viewport.PanGesture2 = new MouseGesture(MouseAction.MiddleClick);
            Viewport.RotateGesture = new MouseGesture(MouseAction.RightClick);
            InitializePositionMapper();
            DrawMapSurface();
            LoadData();
            Viewport.ToolTip = tooltip;

            //Viewport.Camera.LookDirection = new Vector3D(0, 0.00000000001, -1);

        }

        private void ParsePowerEntity(XmlNode node, PowerEntity entity)
        {
            entity.Id = long.Parse(node.SelectSingleNode("Id")?.InnerText ?? string.Empty);
            entity.Name = node.SelectSingleNode("Name")?.InnerText;
            entity.X = double.Parse(node.SelectSingleNode("X")?.InnerText ?? string.Empty);
            entity.Y = double.Parse(node.SelectSingleNode("Y")?.InnerText ?? string.Empty);
        }

        private double GetZPosition(double x, double y, double scale, IEnumerable<IPosition> positions, double offset)
        {
            double z = 0.1;
            var halfExtent = scale / 2;
            var xMin = x - halfExtent;
            var yMin = y - halfExtent;
            var xMax = x + halfExtent;
            var yMax = y + halfExtent;
            foreach (var obj in positions)
            {
                if ((obj.X < xMax) & (obj.X > xMin) & (obj.Y < yMax) & (obj.Y > yMin))
                    z += offset;
            }

            return z;
        }


        private ObjectWrapper Create3DObject(PowerEntity entity, Color color, double scale)
        {
            Utility.ToLatLon(entity.X, entity.Y, 34, out var x, out var y);
            if (mapper.IsInRange(x, y) == false) return null;

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
                         new TranslateTransform3D(position.X,  position.Y, GetZPosition(position.X,position.Y,scale,instantiatedObject,scale))
                    }

                }
            };

            mainModel3DGroup.Children.Add(surfaceModel);

            var newObject = new ObjectWrapper(surfaceModel, entity);
            instantiatedObject.Add(newObject);
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
                var spot = Create3DObject(entity, System.Windows.Media.Color.FromRgb(255, 0, 0), .1);
            }

            foreach (XmlNode node in nodeListNode)
            {
                var entity = new NodeEntity();
                ParsePowerEntity(node, entity);

                var spot = Create3DObject(entity, System.Windows.Media.Color.FromRgb(255, 0, 0), .1);
            }

            foreach (XmlNode node in nodeListSwitch)
            {
                var entity = new SwitchEntity();
                ParsePowerEntity(node, entity);
                var spot = Create3DObject(entity, System.Windows.Media.Color.FromRgb(255, 0, 0), .1);
            }

            foreach (XmlNode item in nodeListLines)
            {
                var line = new LineEntity
                {
                    Id = long.Parse(item.SelectSingleNode("Id")?.InnerText ?? string.Empty),
                    Name = item.SelectSingleNode("Name")?.InnerText,
                    IsUnderground = bool.Parse(item.SelectSingleNode("IsUnderground")?.InnerText ?? string.Empty),
                    LineType = item.SelectSingleNode("LineType")?.InnerText,
                    R = float.Parse(item.SelectSingleNode("R")?.InnerText ?? string.Empty),
                    FirstEnd = long.Parse(item.SelectSingleNode("FirstEnd")?.InnerText ?? string.Empty),
                    SecondEnd = long.Parse(item.SelectSingleNode("SecondEnd")?.InnerText ?? string.Empty),
                    ConductorMaterial = item.SelectSingleNode("ConductorMaterial")?.InnerText,
                    ThermalConstantHeat = long.Parse(item.SelectSingleNode("ThermalConstantHeat")?.InnerText ?? string.Empty),
                    Vertices = new List<Point>()
                };

                foreach (XmlNode pointNode in item.ChildNodes[9].ChildNodes)
                {
                    var xRaw = double.Parse(pointNode.SelectSingleNode("X")?.InnerText ?? string.Empty);
                    var yRaw = double.Parse(pointNode.SelectSingleNode("Y")?.InnerText ?? string.Empty);
                    Utility.ToLatLon(xRaw, yRaw, 34, out var lat, out var lon);

                    line.Vertices.Add(new Point()
                    {
                        X = lat,
                        Y = lon
                    });
                }

                var firstEnd = instantiatedObject.FirstOrDefault(x => x.Entity.Id == line.FirstEnd);
                var secondEnd = instantiatedObject.FirstOrDefault(x => x.Entity.Id == line.SecondEnd);
                if (firstEnd == null || secondEnd == null) continue;

                firstEnd.ConnectionCount++;
                secondEnd.ConnectionCount++;

                Utility.ToLatLon(firstEnd.Entity.X, firstEnd.Entity.Y, 34, out var xFirst, out var yFirst);
                Utility.ToLatLon(secondEnd.Entity.X, secondEnd.Entity.Y, 34, out var xSecond, out var ySecond);

                if (mapper.IsInRange(xFirst, yFirst) == false || mapper.IsInRange(xSecond, ySecond) == false) continue;


                var v1 = mapper.Convert(line.Vertices[0].X, line.Vertices[0].Y);
                var vn = mapper.Convert(line.Vertices[line.Vertices.Count - 1].X, line.Vertices[line.Vertices.Count - 1].Y);



                #region hideTHis

                vertexId = 0;
                var mesh = new MeshGeometry3D();

                CreateLineSegment(firstEnd.X,
                    firstEnd.Y,
                    v1.X,
                    v1.Y,
                    mesh);

                for (int i = 0; i < line.Vertices.Count - 1; i++)
                {
                    var p1 = mapper.Convert(line.Vertices[i].X, line.Vertices[i].Y);
                    var p2 = mapper.Convert(line.Vertices[i + 1].X, line.Vertices[i + 1].Y);

                    CreateLineSegment(p1.X,
                                  p1.Y,
                                  p2.X,
                                  p2.Y,
                                  mesh);
                }

                CreateLineSegment(vn.X,
                    vn.Y,
                    secondEnd.X,
                    secondEnd.Y,
                    mesh);


                var mat = new DiffuseMaterial { Brush = new SolidColorBrush(Colors.DarkRed) };
                var surfaceModel = new GeometryModel3D(mesh, mat)
                {
                    BackMaterial = mat,
                    Transform = new Transform3DGroup()
                    {
                        Children = new Transform3DCollection()
                        {
                            new TranslateTransform3D(0,  0, 0.01)
                        }

                    }
                };
                mainModel3DGroup.Children.Add(surfaceModel);
                var lineComponent = new LineContainer(firstEnd, secondEnd, line, surfaceModel);
                lines.Add(line.Id, lineComponent);
                #endregion

            }
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

            foreach (var obj in instantiatedObject)
            {
                if (obj.ConnectionCount < 3)
                {
                    obj.Model.Material = new DiffuseMaterial { Brush = new SolidColorBrush(Color.FromRgb(255, 127,127)) };
                }
                else if (obj.ConnectionCount >= 3 && obj.ConnectionCount < 5)
                {
                    obj.Model.Material = new DiffuseMaterial { Brush = new SolidColorBrush(Color.FromRgb(255, 70,70)) };

                }
                else
                {
                    obj.Model.Material = new DiffuseMaterial { Brush = new SolidColorBrush(Color.FromRgb(255, 0,0)) };
                }
            }

        }

        private readonly double width = 0.05;
        private int vertexId;
        private void CreateLineSegment(double x1, double y1, double x2, double y2, MeshGeometry3D mesh)
        {
            var dx = x2 - x1;
            var dy = y2 - y1;

            var normal = new Vector3D(-dy, dx, 0);
            normal.Normalize();

            var offset = normal * width;

            var leftSide1 = new Vector3D(x1, y1, 0) - offset;
            var rightSide1 = new Vector3D(x1, y1, 0) + offset;

            var leftSide2 = new Vector3D(x2, y2, 0) - offset;
            var rightSide2 = new Vector3D(x2, y2, 0) + offset;

            mesh.Positions.Add((Point3D)leftSide1);
            mesh.Positions.Add((Point3D)leftSide2);
            mesh.Positions.Add((Point3D)rightSide1);
            mesh.Positions.Add((Point3D)rightSide2);

            mesh.TriangleIndices.Add(vertexId + 0);
            mesh.TriangleIndices.Add(vertexId + 1);
            mesh.TriangleIndices.Add(vertexId + 3);

            mesh.TriangleIndices.Add(vertexId + 0);
            mesh.TriangleIndices.Add(vertexId + 3);
            mesh.TriangleIndices.Add(vertexId + 2);
            vertexId += 4;
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
