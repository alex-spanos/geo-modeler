using System;
using System.Collections.Generic;
using Geometry;
using DataStructures.Rings;

namespace Topography.DataStructures
{
    #region Geometric prototypes implementations

    public class Triangle :
        TrianglePrototype<Triangle, Edge, Vertex, double>
    {
        public List<Vertex> VerticesIncluded { get; protected set; }

        public Triangle() { }

        public Triangle(Vertex vertexA, Vertex vertexB, Vertex vertexC, List<Vertex> verticesToInclude)
            : base(vertexA, vertexB, vertexC)
        {
            VerticesIncluded = verticesToInclude;
        }

        public Triangle(Edge edge, Vertex vertex, int edgePosition, bool initVerticesList)
            : base(edge, vertex, edgePosition)
        {
            if (initVerticesList)
                VerticesIncluded = new List<Vertex>();
        }

        public Triangle(Edge firstEdge, Edge secondEdge, Edge thirdEdge)
            : base(firstEdge, secondEdge, thirdEdge) { }
    }

    public partial class ExpandableEdgesPath :
        EdgesPath
    {
        public List<int> Expandables;

        public ExpandableEdgesPath(List<Vector> path, List<int> expandables)
            : base(path)
        {
            Expandables = expandables;
        }
    }

    public partial class EdgesPath :
        Vector
    {
        public List<Vector> Path;
        /*
        public EdgesPath(Vertex vA, Vertex vB)
            : base(vA, vB)
        {
            Path = new List<Vector>();
        }
        */
        public EdgesPath(List<Vector> path)
            : base(path[path.Count - 1].StartVertex, path[0].EndVertex)
        {
            Path = path;
        }
    }

    public class Edge :
        Vector,
        IEdgePrototype<Triangle, Edge, Vertex, double>
    {
        public Edge NextIn { get; set; }
        public Edge MirrorOut { get; set; }

        public Triangle IncludingTriangle { get; set; }

        public void ConnectWithMirror(Edge edge)
        {
            MirrorOut = edge;
            edge.MirrorOut = this;
        }
    }

    public partial class Vector :
        VectorPrototype<Vertex, double>
    {
        double? _inverseSlope;
        double InverseSlope
        {
            get
            {
                if (!_inverseSlope.HasValue)
                {
                    double dif = StartVertex.Y - EndVertex.Y;
                    if (Math.Abs(dif - 0) > Double.Epsilon)
                        _inverseSlope = (StartVertex.X - EndVertex.X)/dif;
                    else _inverseSlope = double.PositiveInfinity;
                }
                return (double) _inverseSlope;
            }
        }

        double? _midX;
        double MidX
        {
            get
            {
                if (!_midX.HasValue)
                    _midX = (StartVertex.X + EndVertex.X)/2;
                return (double) _midX;
            }
        }

        double? _midY;
        double MidY
        {
            get
            {
                if (!_midY.HasValue)
                    _midY = (StartVertex.Y + EndVertex.Y)/2;
                return (double) _midY;
            }
        }

        public Vector() {}

        public Vector(Vertex vertexA, Vertex vertexB)
            : base(vertexA, vertexB) {}
    }

    public partial class Vertex :
        VertexPrototype<double>
    {
        public override double X { get; set; }
        public override double Y { get; set; }
        public override double Z { get; set; }

        double? _sumOfSquares;
        double SumOfSquares
        {
            get
            {
                if (!_sumOfSquares.HasValue)
                    _sumOfSquares = Math.Pow(X, 2) + Math.Pow(Y, 2);
                return (double)_sumOfSquares;
            }
        }

        public Vertex() { }

        public Vertex(double x, double y, double z)
            : base(x, y, z) { }
    }

    #endregion // End of Geometric prototypes implementations

    #region Generic rings implementations

    public partial class EdgesRing :
        DataRing<Vector>
    {
        readonly List<int> _expandableEdgesPaths;

        public EdgesRing()
        {
            _expandableEdgesPaths = new List<int>();
        }
    }

    #endregion // End of Generic rings implementations

    public class TriangleBranch
    {
        public Triangle Node { get; set; }

        public TriangleBranch PreviousNode { get; set; }

        public TriangleBranch() { }

        public TriangleBranch(Triangle node, TriangleBranch previous)
        {
            Node = node;
            PreviousNode = previous;
        }
    }

    /// <summary>
    /// Orientation of vertex relative to vector.
    /// </summary>
    public enum VertexVectorOrientation
    {
        ToTheLeftOf,
        Colinear,
        ToTheRightOf
    }

    /// <summary>
    /// Vertex to circle relative position.
    /// </summary>
    public enum VertexCircleRelation
    {
        Inside,
        Cocircular,
        Outside
    }
}
