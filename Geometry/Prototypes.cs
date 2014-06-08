using System;
using System.Collections.Generic;
using System.Linq;

namespace Geometry
{
    public class TrianglePrototype<TTriangle, TEdge, TVertex, TData>
        where TTriangle : TrianglePrototype<TTriangle, TEdge, TVertex, TData>
        where TEdge : VectorPrototype<TVertex, TData>, IEdgePrototype<TTriangle, TEdge, TVertex, TData>, new()
        where TVertex : VertexPrototype<TData>
        where TData : IEquatable<TData>
    {
        public TEdge[] Edges { get; set; }

        public TrianglePrototype() {}

        public TrianglePrototype(TVertex vertexA, TVertex vertexB, TVertex vertexC)
        {
            Init(vertexA, vertexB, vertexC);
        }

        public void Init(TVertex vertexA, TVertex vertexB, TVertex vertexC)
        {
            Edges = new[]
            {
                CreateEdge(vertexA, vertexB),
                CreateEdge(vertexB, vertexC),
                CreateEdge(vertexC, vertexA)
            };
            InitializeTriangle();
        }

        public TrianglePrototype(TEdge edge, TVertex vertex, int edgePosition)
        {
            Init(edge, vertex, edgePosition);
        }

        public void Init(TEdge edge, TVertex vertex, int edgePosition)
        {
            edge.IncludingTriangle = (TTriangle) this;
            Edges = new TEdge[3];
            Edges[edgePosition] = edge;
            Edges[(edgePosition + 1)%3] = CreateEdge(edge.EndVertex, vertex);
            Edges[(edgePosition + 2)%3] = CreateEdge(vertex, edge.StartVertex);
            InitializeTriangle();
        }

        public TrianglePrototype(TEdge firstEdge, TEdge secondEdge, TEdge thirdEdge)
        {
            Init(firstEdge, secondEdge, thirdEdge);
        }

        public void Init(TEdge firstEdge, TEdge secondEdge, TEdge thirdEdge)
        {
            if (firstEdge == null)
            {
                firstEdge = CreateEdge(thirdEdge.EndVertex, secondEdge.StartVertex);
                secondEdge.IncludingTriangle = (TTriangle) this;
                thirdEdge.IncludingTriangle = (TTriangle) this;
            }
            else if (secondEdge == null)
            {
                firstEdge.IncludingTriangle = (TTriangle) this;
                secondEdge = CreateEdge(firstEdge.EndVertex, thirdEdge.StartVertex);
                thirdEdge.IncludingTriangle = (TTriangle) this;
            }
            else if (thirdEdge == null)
            {
                firstEdge.IncludingTriangle = (TTriangle) this;
                secondEdge.IncludingTriangle = (TTriangle) this;
                thirdEdge = CreateEdge(secondEdge.EndVertex, firstEdge.StartVertex);
            }
            Edges = new[] {firstEdge, secondEdge, thirdEdge};
            InitializeTriangle();
        }

        TEdge CreateEdge(TVertex vertexA, TVertex vertexB)
        {
            return new TEdge
            {
                StartVertex = vertexA,
                EndVertex = vertexB,
                IncludingTriangle = (TTriangle) this
            };
        }

        void InitializeTriangle()
        {
            Edges[0].NextIn = Edges[1];
            Edges[1].NextIn = Edges[2];
            Edges[2].NextIn = Edges[0];
        }
    }

    public interface IEdgePrototype<TTriangle, TEdge, TVertex, TData>
        where TTriangle : TrianglePrototype<TTriangle, TEdge, TVertex, TData>
        where TEdge : VectorPrototype<TVertex, TData>, IEdgePrototype<TTriangle, TEdge, TVertex, TData>, new()
        where TVertex : VertexPrototype<TData>
        where TData : IEquatable<TData>
    {
        TEdge NextIn { get; set; }
        TEdge MirrorOut { get; set; }

        TTriangle IncludingTriangle { get; set; }

        void ConnectWithMirror(TEdge edge);
    }

    public class VectorPrototype<TVertex, TData>
        where TVertex : VertexPrototype<TData>
        where TData : IEquatable<TData>
    {
        public TVertex StartVertex { get; set; }
        public TVertex EndVertex { get; set; }

        public VectorPrototype() { }

        public VectorPrototype(TVertex vertexA, TVertex vertexB)
        {
            StartVertex = vertexA;
            EndVertex = vertexB;
        }
    }

    public abstract class VertexPrototype<TData>
        where TData : IEquatable<TData>
    {
        public abstract TData X { get; set; }
        public abstract TData Y { get; set; }
        public abstract TData Z { get; set; }

        protected VertexPrototype() {}

        protected VertexPrototype(TData x, TData y, TData z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public bool IsEqualToByRef(VertexPrototype<TData> vertex)
        {
            return ReferenceEquals(this, vertex);
        }

        public bool IsEqualToByVal(VertexPrototype<TData> vertex)
        {
            return X.Equals(vertex.X) && Y.Equals(vertex.Y) && Z.Equals(vertex.Z);
        }

        public bool IsEqualToByVal(TData x, TData y, TData z)
        {
            return X.Equals(x) && Y.Equals(y) && Z.Equals(z);
        }
    }

    public static class SearchTool
    {
        public static TEdge FindVertice<TTriangle, TEdge, TVertex>(this List<TTriangle> triangles, Predicate<TEdge> check)
            where TTriangle : TrianglePrototype<TTriangle, TEdge, TVertex, double>
            where TEdge : VectorPrototype<TVertex, double>, IEdgePrototype<TTriangle, TEdge, TVertex, double>, new()
            where TVertex : VertexPrototype<double>
        {
            int i = 0;
            bool found = false;
            TEdge vertice = null;

            while (!found && i < triangles.Count)
            {
                TTriangle triangle = triangles[i];
                int j = 0;
                do
                {
                    vertice = triangle.Edges[j];
                    found = check(vertice);
                    j++;
                }
                while (!found && j < 3);
                i++;
            }
            return found ? vertice : null;
        }

        public static bool PairExists<TVertex>(this List<Tuple<TVertex, TVertex>> pairs, Tuple<TVertex, TVertex> pair)
            where TVertex : VertexPrototype<double>
        {
            return pairs.Exists(line =>
                (line.Item1.IsEqualToByRef(pair.Item1) && line.Item2.IsEqualToByRef(pair.Item2)) ||
                (line.Item1.IsEqualToByRef(pair.Item2) && line.Item2.IsEqualToByRef(pair.Item1)));
        }

        public static TVertex GetPoint<TVertex>(this List<TVertex> points, TVertex point, bool add)
            where TVertex : VertexPrototype<double>
        {
            if (point == null) return null;
            TVertex pointCol = points.Find(p => p.IsEqualToByVal(point));
            if (add && pointCol == null)
            {
                points.Add(point);
                return point;
            }
            return pointCol;
        }

        #region Min - Max

        public static Tuple<double, double> MinMaxX<TVertex>(this List<TVertex> points)
            where TVertex : VertexPrototype<double>
        {
            return MinMax(points, GetX);
        }

        public static Tuple<double, double> MinMaxY<TVertex>(this List<TVertex> points)
            where TVertex : VertexPrototype<double>
        {
            return MinMax(points, GetY);
        }

        public static Tuple<double, double> MinMaxZ<TVertex>(this List<TVertex> points)
            where TVertex : VertexPrototype<double>
        {
            return MinMax(points, GetZ);
        }

        static Tuple<double, double> MinMax<TVertex>(List<TVertex> points, GetCoordinate<TVertex> coo)
            where TVertex : VertexPrototype<double>
        {
            if (points == null || points.Count == 0)
                return new Tuple<double, double>(0, 0);
            double max, min = max = coo(points[0]);
            if (points.Count == 1)
                return new Tuple<double, double>(min, max);
            foreach (double coord in points.Select(v => coo(v)))
            {
                if (coord < min) min = coord;
                if (coord > max) max = coord;
            }
            return new Tuple<double, double>(min, max);
        }

        delegate double GetCoordinate<in TVertex>(TVertex vertex)
            where TVertex : VertexPrototype<double>;

        static double GetX<TVertex>(TVertex vertex)
            where TVertex : VertexPrototype<double>
        {
            return vertex.X;
        }

        static double GetY<TVertex>(TVertex vertex)
            where TVertex : VertexPrototype<double>
        {
            return vertex.Y;
        }

        static double GetZ<TVertex>(TVertex vertex)
            where TVertex : VertexPrototype<double>
        {
            return vertex.Z;
        }

        #endregion
    }
}
