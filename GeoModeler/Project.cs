using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Geometry;

namespace GeoModeler
{
    public partial class ProjectControl
    {
        #region Fields

        double PointRadious = 0.1;
        static readonly Vector3D[] CubeFrame = new[] {
            new Vector3D(1, -1, 1),
            new Vector3D(1, 1, 1),
            new Vector3D(-1, 1, 1),
            new Vector3D(-1, -1, 1),
            new Vector3D(1, -1, -1),
            new Vector3D(1, 1, -1),
            new Vector3D(-1, 1, -1),
            new Vector3D(-1, -1, -1)
        };

        List<Point3Dw> Points;
        List<Triangle3D> Triangles;
        List<Tuple<Point3Dw, Point3Dw>> Constrains;
        List<Tuple<Point3Dw, Point3Dw>> IsoLines;
        ModelVisual3D VisualModel3D;
        Model3DGroup PointsGroup, TrianglesGroup, ConstrainsGroup, IsolinesGroup;
        bool PointsVisible, TrianglesVisible, ConstrainsVisible, IsolinesVisible, FrameVisible;

        #endregion

        #region Initialization
        /*
        public ProjectControl(List<Point3Dw> points, List<Triangle3D> triangles)
        {
            Points = points;
            Triangles = triangles;
        }
        */
        public ProjectControl(string fileName)
        { InitFromFile(fileName); }

        public void InitFromFile(string fileName)
        {
            bool error = false;
            string[] inputLines = null;
            try { inputLines = System.IO.File.ReadAllLines(fileName); }
            catch { error = true; }
            if (!error) Triangles = Formater.en.ReadTriangles(inputLines, out Points, 0, inputLines.Length - 1);
        }

        #endregion

        public void Show3DModel()
        {
            Point3D propPosition;
            Vector3D propLookDirection;

            VisualModel3D = Create3DModel(out propPosition, out propLookDirection);
            model3DControl.projectView.Children.Add(VisualModel3D);
            model3DControl.cameraControl.MoveTo(propPosition);
            model3DControl.cameraControl.RotateTo(propLookDirection);
        }

        ModelVisual3D Create3DModel(out Point3D position, out Vector3D lookDirection)
        {
            ModelVisual3D model = new ModelVisual3D();
            Model3DGroup group = new Model3DGroup();
            DirectionalLight light;

            PointsGroup = CreatePoints3DGroup();
            group.Children.Add(PointsGroup);

            TrianglesGroup = CreateTriangles3DGroup();
            group.Children.Add(TrianglesGroup);

            light = new DirectionalLight(Colors.White, new Vector3D(0, 0, -1));
            group.Children.Add(light);

            position = new Point3D(group.Bounds.Location.X, group.Bounds.Location.Y, group.Bounds.Location.Z + group.Bounds.SizeZ);
            lookDirection = new Vector3D(-group.Bounds.SizeX, -group.Bounds.SizeY, -group.Bounds.SizeZ);

            model.Content = group;

            return model;
        }

        Model3DGroup CreatePoints3DGroup()
        {
            int i, pos = 0;
            Model3DGroup group = new Model3DGroup();
            GeometryModel3D geometry;
            MeshGeometry3D mesh = new MeshGeometry3D();
            Material material = new DiffuseMaterial(new SolidColorBrush(Colors.Blue));
            Vector3D[] frame = new Vector3D[8];

            for (i = 0; i < 8; i++) frame[i] = PointRadious * CubeFrame[i];
            for (i = 0; i < Points.Count; i++) AddCube(mesh, ref pos, frame, Points[i]._);
            geometry = new GeometryModel3D(mesh, material);
            group.Children.Add(geometry);

            return group;
        }

        Model3DGroup CreateTriangles3DGroup()
        {
            int i, k = 0;
            Model3DGroup group = new Model3DGroup();
            GeometryModel3D geometry;
            MeshGeometry3D mesh = new MeshGeometry3D();
            Material material = new DiffuseMaterial(new SolidColorBrush(Colors.DarkKhaki));
            Triangle3D triangle;
            Point3D[] points = new Point3D[3];

            for (i = 0; i < Triangles.Count; i++)
            {
                triangle = Triangles[i];
                points[0] = triangle.Edges[0].StartVertex._;
                points[1] = triangle.Edges[1].StartVertex._;
                points[2] = triangle.Edges[2].StartVertex._;
                AddTriangle(mesh, ref k, points);
            }
            geometry = new GeometryModel3D(mesh, material);
            group.Children.Add(geometry);

            return group;
        }

        void AddCube(MeshGeometry3D mesh, ref int pos, Vector3D[] frame, Point3D center)
        {
            Point3D[] points = new Point3D[8];

            for (int i = 0; i < 8; i++) points[i] = center + frame[i];
            AddRentagle(mesh, ref pos, new Point3D[4] { points[0], points[1], points[2], points[3] });
            //AddRentagle(mesh, ref pos, new Point3D[4] { points[7], points[6], points[5], points[4] });
            AddRentagle(mesh, ref pos, new Point3D[4] { points[0], points[4], points[5], points[1] });
            AddRentagle(mesh, ref pos, new Point3D[4] { points[1], points[5], points[6], points[2] });
            AddRentagle(mesh, ref pos, new Point3D[4] { points[2], points[6], points[7], points[3] });
            AddRentagle(mesh, ref pos, new Point3D[4] { points[3], points[7], points[4], points[0] });
        }

        void AddRentagle(MeshGeometry3D mesh, ref int pos, Point3D[] points)
        {
            AddTriangle(mesh, ref pos, new Point3D[3] { points[0], points[1], points[2] });
            AddTriangle(mesh, ref pos, new Point3D[3] { points[0], points[2], points[3] });
        }

        void AddTriangle(MeshGeometry3D mesh, ref int pos, Point3D[] points)
        {
            int i;
            Vector3D normal = Normal(points);

            for (i = 0; i < 3; i++)
            {
                mesh.Positions.Add(points[i]);
                mesh.TriangleIndices.Add(pos + i);
                mesh.Normals.Add(normal);
            }
            pos += 3;
        }

        Vector3D Normal(Point3D[] points)
        {
            Vector3D vectorAB = new Vector3D(points[1].X - points[0].X,
                                             points[1].Y - points[0].Y,
                                             points[1].Z - points[0].Z),
                     vectorAC = new Vector3D(points[2].X - points[0].X,
                                             points[2].Y - points[0].Y,
                                             points[2].Z - points[0].Z);

            return Vector3D.CrossProduct(vectorAB, vectorAC);
        }
    }

    class Triangle3D : TrianglePrototype<Triangle3D, Edge3D, Point3Dw, double>
    {
        public Triangle3D() { }

        public Triangle3D(Point3Dw vertexA, Point3Dw vertexB, Point3Dw vertexC) : base(vertexA, vertexB, vertexC) { }
    }

    class Edge3D : VectorPrototype<Point3Dw, double>, IEdgePrototype<Triangle3D, Edge3D, Point3Dw, double>
    {
        public Edge3D NextIn { get; set; }
        public Edge3D MirrorOut { get; set; }

        public Triangle3D IncludingTriangle { get; set; }

        public void ConnectWithMirror(Edge3D edge)
        {
            MirrorOut = edge;
            edge.MirrorOut = this;
        }
    }

    class Point3Dw : VertexPrototype<double>
    {
        public Point3D _;

        public Point3Dw()
        { _ = new Point3D(); }

        public override double X { get { return _.X; } set { _.X = value; } }
        public override double Y { get { return _.Y; } set { _.Y = value; } }
        public override double Z { get { return _.Z; } set { _.Z = value; } }
    }

    static class Formater
    { public static SimpleFormatter<Triangle3D, Edge3D, Point3Dw>
        en = new SimpleFormatter<Triangle3D, Edge3D, Point3Dw>("en-US"); }
}
