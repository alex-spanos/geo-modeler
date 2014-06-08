using System;
using System.Collections.Generic;

namespace Topography.DataStructures
{
    #region Geometric implementations functionality

    public partial class ExpandableEdgesPath
    {
        public void FlattenPath()
        {
            bool more = Expandables.Count != 0;

            while (more)
            {
                int index = Expandables[0] - 1;
                var path = (ExpandableEdgesPath)Path[index];
                path.FlattenPath();
                Expandables.RemoveAt(0);
                more = Expandables.Count != 0;
                Path.RemoveAt(index);
                Path.InsertRange(index, path.Path);
                if (more)
                    for (int j = 0; j < Expandables.Count; j++)
                        if (Expandables[j] > index + 1)
                            Expandables[j] += path.Path.Count - 1;
            }
        }
    }

    public partial class EdgesPath
    {
        public Edge Triangulate(List<Triangle> triangles)
        {
            return TriangulatePath(this, triangles);
        }

        Edge TriangulatePath(EdgesPath path, List<Triangle> triangles)
        {
            int index = 0;
            Triangle triangle;
            Edge edge1;

            double minRadious = path.RightArcHeight(path.Path[0].StartVertex);
            for (int i = 1; i < path.Path.Count - 1; i++)
            {
                double radious = path.RightArcHeight(path.Path[i].StartVertex);
                if (!(radious < minRadious)) continue;
                minRadious = radious;
                index = i;
            }
            if (index > 0)
            {
                if (index < path.Path.Count - 2)
                {
                    triangle = new Triangle(path.EndVertex, path.StartVertex, path.Path[index].StartVertex, null);
                    TriangulatePath(new EdgesPath(path.Path.GetRange(0, index + 1)), triangles).
                        ConnectWithMirror(triangle.Edges[2]);
                    TriangulatePath(new EdgesPath(path.Path.GetRange(index + 1, path.Path.Count - index - 1)), triangles).
                        ConnectWithMirror(triangle.Edges[1]);
                }
                else
                {
                    edge1 = (Edge)path.Path[path.Path.Count - 1];
                    triangles.Remove(edge1.IncludingTriangle);
                    triangle = new Triangle(edge1, path.Path[0].EndVertex, 1, false);
                    TriangulatePath(new EdgesPath(path.Path.GetRange(0, index + 1)), triangles).
                        ConnectWithMirror(triangle.Edges[2]);
                }
            }
            else
            {
                Edge edge2;
                if (index < path.Path.Count - 2)
                {
                    edge2 = (Edge)path.Path[0];
                    triangles.Remove(edge2.IncludingTriangle);
                    triangle = new Triangle(edge2, path.Path[path.Path.Count - 1].StartVertex, 2, false);
                    triangle.Edges[1].ConnectWithMirror(TriangulatePath(
                        new EdgesPath(path.Path.GetRange(index + 1, path.Path.Count - index - 1)), triangles));
                }
                else
                {
                    edge1 = (Edge)path.Path[1];
                    edge2 = (Edge)path.Path[0];
                    triangles.Remove(edge1.IncludingTriangle);
                    triangles.Remove(edge2.IncludingTriangle);
                    triangle = new Triangle(null, edge1, edge2);
                }
            }
            triangles.Add(triangle);

            return triangle.Edges[0];
        }
    }

    public partial class Vector
    {
        public double LeftArcHeight(Vertex vertex)
        {
            return ArcHeight(vertex, VertexVectorOrientation.ToTheLeftOf);
        }

        public double RightArcHeight(Vertex vertex)
        {
            return ArcHeight(vertex, VertexVectorOrientation.ToTheRightOf);
        }

        public double ArcHeight(Vertex vertex, VertexVectorOrientation orientation)
        {
            Vertex o = CircleCenter(vertex);

            double r = Math.Sqrt(Math.Pow(o.X - StartVertex.X, 2) + Math.Pow(o.Y - StartVertex.Y, 2)),
                   d = Math.Sqrt(Math.Pow(o.X - MidX, 2) + Math.Pow(o.Y - MidY, 2));

            if (o.IsRelativeToLine(this) == orientation) return r + d;
            return r - d;
        }

        Vertex CircleCenter(Vertex vertex)
        {
            Vector ab, ac;

            if (!double.IsPositiveInfinity(InverseSlope))
            {
                ac = new Vector(StartVertex, vertex);
                if (double.IsPositiveInfinity(ac.InverseSlope))
                {
                    ab = new Vector(EndVertex, StartVertex);
                    ac = new Vector(EndVertex, vertex);
                }
                else ab = this;
            }
            else
            {
                ab = new Vector(vertex, StartVertex);
                ac = new Vector(vertex, EndVertex);
            }

            double xO = (ac.MidX * ac.InverseSlope - ab.MidX * ab.InverseSlope + ac.MidY - ab.MidY) /
                        (ac.InverseSlope - ab.InverseSlope),
                   yO = ab.InverseSlope * (ab.MidX - xO) + ab.MidY;

            return new Vertex(xO, yO, 0);
        }
    }

    public partial class Vertex
    {
        public VertexVectorOrientation IsRelativeToLine(Vector vector)
        {
            double result = (new Matrix(new[]
            {
                new[]
                {
                    vector.EndVertex.X - vector.StartVertex.X,
                    vector.EndVertex.Y - vector.StartVertex.Y
                },
                new[]
                {
                    X - vector.StartVertex.X,
                    Y - vector.StartVertex.Y
                }
            })).Determinant();

            return Math.Abs(result - 0) < Double.Epsilon
                       ? VertexVectorOrientation.Colinear
                       : result > 0
                             ? VertexVectorOrientation.ToTheLeftOf
                             : VertexVectorOrientation.ToTheRightOf;
        }

        public VertexCircleRelation IsRelativeToCircle(Vertex a, Vertex b, Vertex c)
        {
            double result = (new Matrix(new[]
            {
                new[] {1, a.X, a.Y, a.SumOfSquares},
                new[] {1, b.X, b.Y, b.SumOfSquares},
                new[] {1, c.X, c.Y, c.SumOfSquares},
                new[] {1, X, Y, SumOfSquares}
            })
            ).Determinant();

            return Math.Abs(result - 0) < Double.Epsilon
                       ? VertexCircleRelation.Cocircular
                       : result > 0
                             ? VertexCircleRelation.Outside
                             : VertexCircleRelation.Inside;
        }

        public double DistanceFromLine(Vector v)
        {
            double dX = v.EndVertex.X - v.StartVertex.X,
                   dY = v.EndVertex.Y - v.StartVertex.Y;

            return Math.Abs(dX * (v.StartVertex.Y - Y) - (v.StartVertex.X - X) * dY) /
                   Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));
        }

        class Matrix
        {
            private double[][] Data { get; set; }

            public Matrix(double[][] data)
            {
                InitializeMatrix(data, false);
            }

            public Matrix(double[][] data, bool check)
            {
                InitializeMatrix(data, check);
            }

            void InitializeMatrix(double[][] data, bool check)
            {
                if (!check) Data = data;
                else if (SquareStructure(data)) Data = data;
            }

            bool SquareStructure(double[][] data)
            {
                int i = 0;
                bool noError = true;

                if (data != null)
                {
                    int dim = data.Length;
                    if (dim >= 0)
                        do
                        {
                            if (dim != data[i].Length) noError = false;
                            i++;
                        }
                        while (noError && i < data.Length);
                    else noError = false;
                }
                else noError = false;
                return noError;
            }

            public double Determinant()
            {
                double result = 0D;
                int subDim = Data.Length - 1;
                bool positive = true;

                switch (subDim)
                {
                    case 0:
                        result = Data[0][0];
                        break;
                    case 1:
                        result = Data[0][0] * Data[1][1] - Data[0][1] * Data[1][0];
                        break;
                    default:
                        for (int i = 0; i <= subDim; i++)
                        {
                            var subData = new double[subDim][];
                            int j, k;
                            for (j = 0; j < i; j++)
                            {
                                subData[j] = new double[subDim];
                                for (k = 0; k < subDim; k++)
                                    subData[j][k] = Data[j][k + 1];
                            }
                            for (j = i; j < subDim; j++)
                            {
                                subData[j] = new double[subDim];
                                for (k = 0; k < subDim; k++)
                                    subData[j][k] = Data[j + 1][k + 1];
                            }
                            var subMatrix = new Matrix(subData);
                            result = positive ? result + Data[i][0] * subMatrix.Determinant()
                                              : result - Data[i][0] * subMatrix.Determinant();
                            positive = !positive;
                        }
                        break;
                }
                return result;
            }
        }
    }

    #endregion // End of Geometric implementations functionality

    #region Rings implementations functionality

    public partial class EdgesRing
    {
        public void Triangulate(List<Triangle> triangles)
        {
            for (int i = 1; i <= Size; i++)
            {
                if (_expandableEdgesPaths.Contains(i))
                {
                    var epPart = (ExpandableEdgesPath)this[i];
                    epPart.FlattenPath();
                    epPart.Triangulate(triangles);
                }
                else
                {
                    var ePart = (Edge)this[i];
                    ePart.MirrorOut.MirrorOut = null;
                    triangles.Remove(ePart.IncludingTriangle);
                }
            }
        }

        public void CurvePath()
        {
            int lastChange = Size, index = 1;
            bool more = true;

            while (more)
            {
                int pos = index;
                GetNextConvexPath(ref index);
                if (index > pos + 1)
                {
                    lastChange = NormalizeIndex(pos);
                    JoinEdges(pos, index);
                    index = pos;
                }
                else more = NormalizeIndex(index) != lastChange;
            }
        }

        void GetNextConvexPath(ref int index)
        {
            bool more = true;
            Vector firstEdge = this[index];

            index++;
            while (more)
            {
                Vector secondEdge = this[index];
                if (firstEdge.EndVertex.IsRelativeToLine(secondEdge)
                    == VertexVectorOrientation.ToTheLeftOf)
                {
                    index++;
                    firstEdge = secondEdge;
                }
                else more = false;
            }
        }

        void JoinEdges(int e1, int e2)
        {
            int dif = e2 - e1, i;
            int k = NormalizeIndex(e1),
                l = NormalizeIndex(e2 - 1);

            var path = new List<Vector>(dif);
            var expandables = new List<int>(dif);

            for (i = e1; i < e2; i++)
            {
                int j = NormalizeIndex(i);
                path.Add(this[j]);
                if (_expandableEdgesPaths.Contains(j))
                {
                    expandables.Add(j - k + 1);
                    _expandableEdgesPaths.Remove(j);
                }
            }

            if (l < k)
            {
                for (i = k; i <= Size; i++) DeleteNode(k);
                for (i = 1; i <= l; i++) DeleteNode(1);
                AddDataToEnd(new ExpandableEdgesPath(path, expandables));
            }
            else
            {
                for (i = k; i <= l; i++) DeleteNode(k);
                if (k == Size + 1) AddDataToEnd(new ExpandableEdgesPath(path, expandables));
                else InsertData(new ExpandableEdgesPath(path, expandables), k);
            }

            _expandableEdgesPaths.Add(k);
            dif--;
            for (i = 0; i < _expandableEdgesPaths.Count; i++)
                if (_expandableEdgesPaths[i] > l)
                    _expandableEdgesPaths[i] -= dif;
        }
    }

    #endregion // End of Rings implementations functionality
}
