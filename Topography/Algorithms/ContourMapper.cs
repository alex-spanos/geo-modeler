using System;
using System.Collections.Generic;
using Geometry;
using Synchronization.Core;
using Synchronization.Models;
using Topography.DataStructures;

namespace Topography.Algorithms
{
    public class ContourMapper :
        IWorker<ContourMapperTicketIn, ContourMapperTicketOut>
    {
        List<Vertex> _points;

        List<Triangle> _triangles;

        List<double> _isoLinesHeights;

        List<List<Tuple<Vertex, Vertex>>> _isoLines;

        public IPauseable Fiber { get; set; }

        public ContourMapper() {}

        public ContourMapper(IPauseable fiber)
        {
            Fiber = fiber;
        }

        public ContourMapperTicketOut DoWork(ContourMapperTicketIn ticket)
        {
            return new ContourMapperTicketOut(IsoLinesOf(ticket.Points, ticket.Triangles, ticket.IsoDimention));
        }

        public List<List<Tuple<Vertex, Vertex>>> IsoLinesOf(List<Vertex> points, List<Triangle> triangles, double isoDimention)
        {
            _isoLines = new List<List<Tuple<Vertex, Vertex>>>();
            _points = points;
            _triangles = triangles;

            if (_triangles != null && _triangles.Count > 0 &&
                _points != null && _points.Count > 0 && isoDimention > 0)
            {
                Tuple<double, double> minmaxZ = _points.MinMaxZ();
                var isoLinesNumber = (int)Math.Floor((minmaxZ.Item2 - minmaxZ.Item1) / isoDimention);
                if (isoLinesNumber > 0)
                {
                    _isoLinesHeights = new List<double>(isoLinesNumber);
                    double height = minmaxZ.Item1;
                    for (int i = 0; i < isoLinesNumber; i++)
                    {
                        height += isoDimention;
                        _isoLinesHeights.Add(height);
                    }
                    IsoLine();
                }
            }

            List<List<Tuple<Vertex, Vertex>>> isoLines = _isoLines;
            _isoLines = null;
            _points = null;
            _triangles = null;
            return isoLines;
        }

        void IsoLine()
        {
            foreach (double isoLineHeight in _isoLinesHeights)
            {
                List<Tuple<Vertex, Vertex>> isoLine = CreateIsoLine(isoLineHeight);
                if (isoLine != null) _isoLines.Add(isoLine);
            }
        }

        List<Tuple<Vertex, Vertex>> CreateIsoLine(double isoLineHeight)
        {
            var isoLine = new List<Tuple<Vertex, Vertex>>();
            int i = 0;

            while (i < _triangles.Count)
            {
                Triangle triangle = _triangles[i];
                IsolineTriangleIntersection triangleStatus;
                double maxZ;
                Tuple<Vertex, Vertex> pair = FindIntersections(triangle, isoLineHeight, out triangleStatus, out maxZ);
                if (triangleStatus == IsolineTriangleIntersection.FlatTriangle ||
                    maxZ <= isoLineHeight)
                    _triangles.Remove(triangle);
                else
                {
                    if (triangleStatus == IsolineTriangleIntersection.PairOfPoints)
                        isoLine.Add(pair);
                    i++;
                }
            }
            return isoLine.Count > 0 ? isoLine : null;
        }

        static Tuple<Vertex, Vertex> FindIntersections(Triangle triangle, double isoLineHeight,
                                 out IsolineTriangleIntersection triangleStatus, out double maxZ)
        {
            double maxA, maxB, maxC;
            IsolineEdgeIntersection psA, psB, psC;

            Vertex pointA = FindIntersection(triangle.Edges[0], isoLineHeight, out psA, out maxA),
                   pointB = FindIntersection(triangle.Edges[1], isoLineHeight, out psB, out maxB),
                   pointC = FindIntersection(triangle.Edges[2], isoLineHeight, out psC, out maxC);

            maxZ = Math.Max(maxA, Math.Max(maxB, maxC));
            triangleStatus = IsolineTriangleIntersection.NoSection;

            if (psA == IsolineEdgeIntersection.FlatLine)
            {
                if (psB == IsolineEdgeIntersection.FlatLine ||
                    psC == IsolineEdgeIntersection.FlatLine)
                {
                    triangleStatus = IsolineTriangleIntersection.FlatTriangle;
                    return null;
                }
                return new Tuple<Vertex, Vertex>(triangle.Edges[0].StartVertex, triangle.Edges[0].EndVertex);
            }
            if (psB == IsolineEdgeIntersection.FlatLine)
            {
                if (psA == IsolineEdgeIntersection.FlatLine ||
                    psC == IsolineEdgeIntersection.FlatLine)
                {
                    triangleStatus = IsolineTriangleIntersection.FlatTriangle;
                    return null;
                }
                return new Tuple<Vertex, Vertex>(triangle.Edges[1].StartVertex, triangle.Edges[1].EndVertex);
            }
            if (psC == IsolineEdgeIntersection.FlatLine)
            {
                if (psA == IsolineEdgeIntersection.FlatLine ||
                    psB == IsolineEdgeIntersection.FlatLine)
                {
                    triangleStatus = IsolineTriangleIntersection.FlatTriangle;
                    return null;
                }
                return new Tuple<Vertex, Vertex>(triangle.Edges[2].StartVertex, triangle.Edges[2].EndVertex);
            }

            if (psA != IsolineEdgeIntersection.NoSection)
            {
                if (psA == IsolineEdgeIntersection.EndPoint &&
                    psB == IsolineEdgeIntersection.StartPoint)
                {
                    if (psC != IsolineEdgeIntersection.NoSection)
                    {
                        triangleStatus = IsolineTriangleIntersection.PairOfPoints;
                        return new Tuple<Vertex, Vertex>(pointA, pointC);
                    }
                    triangleStatus = IsolineTriangleIntersection.SinglePoint;
                    return new Tuple<Vertex, Vertex>(pointA, null);
                }
                if (psA == IsolineEdgeIntersection.StartPoint &&
                    psC == IsolineEdgeIntersection.EndPoint)
                {
                    if (psB != IsolineEdgeIntersection.NoSection)
                    {
                        triangleStatus = IsolineTriangleIntersection.PairOfPoints;
                        return new Tuple<Vertex, Vertex>(pointA, pointB);
                    }
                    triangleStatus = IsolineTriangleIntersection.SinglePoint;
                    return new Tuple<Vertex, Vertex>(pointA, null);
                }
                if (psB != IsolineEdgeIntersection.NoSection)
                {
                    triangleStatus = IsolineTriangleIntersection.PairOfPoints;
                    return new Tuple<Vertex, Vertex>(pointA, pointB);
                }
                if (psC != IsolineEdgeIntersection.NoSection)
                {
                    triangleStatus = IsolineTriangleIntersection.PairOfPoints;
                    return new Tuple<Vertex, Vertex>(pointA, pointC);
                }
                triangleStatus = IsolineTriangleIntersection.SinglePoint;
                return new Tuple<Vertex, Vertex>(pointA, null);
            }
            if (psB != IsolineEdgeIntersection.NoSection)
            {
                if (psB == IsolineEdgeIntersection.EndPoint &&
                    psC == IsolineEdgeIntersection.StartPoint)
                {
                    triangleStatus = IsolineTriangleIntersection.SinglePoint;
                    return new Tuple<Vertex, Vertex>(pointB, null);
                }
                if (psC != IsolineEdgeIntersection.NoSection)
                {
                    triangleStatus = IsolineTriangleIntersection.PairOfPoints;
                    return new Tuple<Vertex, Vertex>(pointB, pointC);
                }
                triangleStatus = IsolineTriangleIntersection.SinglePoint;
                return new Tuple<Vertex, Vertex>(pointB, null);
            }
            if (psC != IsolineEdgeIntersection.NoSection)
            {
                triangleStatus = IsolineTriangleIntersection.SinglePoint;
                return new Tuple<Vertex, Vertex>(pointC, null);
            }
            return null;
        }

        static Vertex FindIntersection(Edge vertice, double isoLineHeight,
              out IsolineEdgeIntersection pointStatus, out double maxZ)
        {
            maxZ = Math.Max(vertice.StartVertex.Z, vertice.EndVertex.Z);
            pointStatus = IsolineEdgeIntersection.NoSection;

            if (Math.Abs(vertice.StartVertex.Z - isoLineHeight) < Double.Epsilon)
            {
                if (Math.Abs(vertice.EndVertex.Z - isoLineHeight) < Double.Epsilon)
                {
                    pointStatus = IsolineEdgeIntersection.FlatLine;
                    return null;
                }
                pointStatus = IsolineEdgeIntersection.StartPoint;
                return vertice.StartVertex;
            }
            if (Math.Abs(vertice.EndVertex.Z - isoLineHeight) < Double.Epsilon)
            {
                if (Math.Abs(vertice.StartVertex.Z - isoLineHeight) < Double.Epsilon)
                {
                    pointStatus = IsolineEdgeIntersection.FlatLine;
                    return null;
                }
                pointStatus = IsolineEdgeIntersection.EndPoint;
                return vertice.EndVertex;
            }

            double zdif = vertice.EndVertex.Z - vertice.StartVertex.Z;

            if (zdif > 0)
            {
                if (vertice.StartVertex.Z < isoLineHeight &&
                    isoLineHeight < vertice.EndVertex.Z)
                    pointStatus = IsolineEdgeIntersection.Between;
            }
            else if (vertice.EndVertex.Z < isoLineHeight &&
                     isoLineHeight < vertice.StartVertex.Z)
                pointStatus = IsolineEdgeIntersection.Between;

            if (pointStatus != IsolineEdgeIntersection.Between) return null;

            double t = (isoLineHeight - vertice.StartVertex.Z) / zdif;

            return new Vertex(vertice.StartVertex.X + (vertice.EndVertex.X - vertice.StartVertex.X)*t,
                              vertice.StartVertex.Y + (vertice.EndVertex.Y - vertice.StartVertex.Y)*t,
                              isoLineHeight);
        }

        enum IsolineTriangleIntersection
        {
            NoSection,
            SinglePoint,
            PairOfPoints,
            FlatTriangle
        }

        enum IsolineEdgeIntersection
        {
            NoSection,
            Between,
            StartPoint,
            EndPoint,
            FlatLine
        }
        /*
        void ConnectTriangles()
        {
            bool stop = false;
            Triangle triangle;
            Vertice mirrorVertice;
            List<Triangle> connectedTriangles = new List<Triangle>(Triangles.Count);

            while (!stop)
            {
                triangle = Triangles[0];
                foreach (Vertice vertice in triangle.Vertices)
                {
                    mirrorVertice = FindVerticeMirrorTo(vertice);
                    if (mirrorVertice != null) vertice.ConnectWithMirror(mirrorVertice);
                }
                Triangles.Remove(triangle);
                connectedTriangles.Add(triangle);
                if (Triangles.Count == 0) stop = true;
            }
            Triangles = connectedTriangles;
        }
        
        Vertice FindVerticeMirrorTo(Vertice vertice)
        {
            return Triangles.FindVertice(new Predicate<Vertice>(v =>
                v.StartPoint.IsEqualToByVal(vertice.EndPoint) &&
                v.EndPoint.IsEqualToByVal(vertice.StartPoint)));
        }*/
    }

    public class ContourMapperTicketIn
    {
        [Channel]
        public readonly List<Vertex> Points;
        [Channel]
        public readonly List<Triangle> Triangles;
        [Channel]
        public readonly double IsoDimention;

        public ContourMapperTicketIn(List<Vertex> points, List<Triangle> triangles, double isoDimention)
        {
            Points = points;
            Triangles = triangles;
            IsoDimention = isoDimention;
        }
    }

    public class ContourMapperTicketOut
    {
        [Channel]
        public readonly List<List<Tuple<Vertex, Vertex>>> IsoLines;

        public ContourMapperTicketOut(List<List<Tuple<Vertex, Vertex>>> isoLines)
        {
            IsoLines = isoLines;
        }
    }
}
