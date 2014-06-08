using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Synchronization.Core;
using Synchronization.Models;
using Topography.DataStructures;

namespace Topography.Algorithms
{
    public class Triangulator :
        IWorker<TriangulatorTicketIn, TriangulatorTicketOut>
    {
        const double RadiousIncreaseByPercent = 0.05;

        #region Fields

        readonly YdimComparer _yComparer = new YdimComparer();
        readonly PointComparer _pComparer = new PointComparer();
        List<Vertex> _points;
        List<Tuple<Vertex, Vertex>> _constrains;
        List<Triangle> _triangles;
        Triangle _rightExtremeTriangle;
        Vertex _k, _l, _m;

        #endregion // End of Fields

        public IPauseable Fiber { get; set; }

        public Triangulator() {}

        public Triangulator(IPauseable fiber)
        {
            Fiber = fiber;
        }

        public TriangulatorTicketOut DoWork(TriangulatorTicketIn ticket)
        {
            return new TriangulatorTicketOut(TriangulationOf(ticket.Points, ticket.Constrains));
        }

        public List<Triangle> TriangulationOf(List<Vertex> points, List<Tuple<Vertex, Vertex>> constrains)
        {
            _triangles = new List<Triangle>();
            _points = points;
            _constrains = constrains;

            if (!(_points == null || _points.Count < 3))
                Triangulate();

            List<Triangle> triangles = _triangles;
            _triangles = null;
            _points = null;
            _constrains = null;
            return triangles;
        }

        void Triangulate()
        {
            #region Local Fields

            Edge firstVertice, secondVertice = null;
            TriangleBranch containingTriangleRoot = new TriangleBranch(),
                           mirrorContainingTriangleRoot = new TriangleBranch();
            var boundary = new List<Tuple<Edge, TriangleBranch>>();
            Tuple<Edge, Edge> adjacentVertices = null;

            #endregion // End of Local Fields

            #region Rearrange data (I)

            _points.Sort(_yComparer);

            double xmin = _points.Min(p => p.X),
                   xmax = _points.Max(p => p.X),
                   ymin = _points[0].Y,
                   ymax = _points[_points.Count - 1].Y,
                   xo = (xmax + xmin)/2D,
                   yo = (ymax + ymin)/2D,
                   xdis = Math.Floor(xo),
                   ydis = Math.Floor(yo);

            foreach (Vertex point in _points)
            {
                point.X -= xdis;
                point.Y -= ydis;
            }

            xmin -= xdis;
            ymin -= ydis;
            xo -= xdis;
            yo -= ydis;

            #endregion // End of Rearrange data (I)

            #region Create container triangle

            double radious = (RadiousIncreaseByPercent + 1)*Math.Sqrt(Math.Pow(xo - xmin, 2) + Math.Pow(yo - ymin, 2)),
                   diam = 2*radious,
                   height = radious*Math.Sqrt(3);

            _k = new Vertex(xo, yo + diam, 0);
            _l = new Vertex(xo - height, yo - radious, 0);
            _m = new Vertex(xo + height, yo - radious, 0);

            _triangles.Add(new Triangle(_k, _l, _m, new List<Vertex>(_points)));
            _rightExtremeTriangle = _triangles[0];

            #endregion // End of Create container triangle

            #region Construct the mesh of points

            foreach (Vertex point in _points)
            {
                Triangle containingTriangle = TriangleIncluding(point);
                containingTriangle.VerticesIncluded.Remove(point);
                containingTriangleRoot.Node = containingTriangle;
                containingTriangleRoot.PreviousNode = null;

                int i, j = i = 0;
                bool found = false;
                do
                {
                    if (containingTriangle.Edges[i].EndVertex == _k)
                        found = true;
                    else i++;
                }
                while (!found);

                found = false;
                do
                {
                    if (point.IsRelativeToLine(containingTriangle.Edges[j])
                        == VertexVectorOrientation.Colinear)
                        found = true;
                    else j++;
                }
                while (!found && j < 3);

                if (found)
                {
                    for (int k = i; k < i + 3; k++) if (k == j)
                    {
                        if (containingTriangle.Edges[j].MirrorOut != null)
                        {
                            firstVertice = containingTriangle.Edges[j].MirrorOut;
                            CreateLocalTriangulationsBoundary(boundary, containingTriangleRoot, firstVertice.NextIn, point);
                            mirrorContainingTriangleRoot.Node = firstVertice.IncludingTriangle;
                            mirrorContainingTriangleRoot.PreviousNode = null;
                            CreateLocalTriangulationsBoundary(boundary, mirrorContainingTriangleRoot, firstVertice.NextIn.NextIn, point);
                            _triangles.Remove(firstVertice.IncludingTriangle);
                        }
                        else boundary.Add(new Tuple<Edge, TriangleBranch>(containingTriangle.Edges[j], containingTriangleRoot));
                    }
                    else CreateLocalTriangulationsBoundary(boundary, containingTriangleRoot, containingTriangle.Edges[k % 3], point);
                }
                else for (j = i; j < i + 3; j++)
                    CreateLocalTriangulationsBoundary(boundary, containingTriangleRoot, containingTriangle.Edges[j % 3], point);
                _triangles.Remove(containingTriangle);

                Tuple<Edge, Edge> previousAdjacentVertices = AddSlice(boundary[0], point);
                firstVertice = previousAdjacentVertices.Item1;
                for (i = 1; i < boundary.Count; i++)
                {
                    adjacentVertices = AddSlice(boundary[i], point);
                    adjacentVertices.Item1.ConnectWithMirror(previousAdjacentVertices.Item2);
                    previousAdjacentVertices = adjacentVertices;
                }
                adjacentVertices.Item2.ConnectWithMirror(firstVertice);

                boundary.Clear();
            }

            #endregion // End of Construct the mesh of points

            #region Envelope in closed curved boundary

            EdgesRing closedBoundary = GetTriangulationsClosedBoundary();
            closedBoundary.CurvePath();
            closedBoundary.Triangulate(_triangles);

            #endregion // End of Envelope in closed curved boundary

            #region Apply edge constrains

            if (!(_constrains == null || _constrains.Count == 0))
            {
                foreach (Tuple<Vertex, Vertex> constrain in _constrains)
                {
                    if (!_points.Contains(constrain.Item1, _pComparer) ||
                        !_points.Contains(constrain.Item2, _pComparer)) continue;
                    List<Tuple<EdgesPath, EdgesPath>> sequencesPairs = GetSequences(constrain.Item1, constrain.Item2);
                    if (sequencesPairs == null) continue;
                    foreach (Tuple<EdgesPath, EdgesPath> pair in sequencesPairs)
                    {
                        firstVertice = null;
                        if (pair.Item1.Path.Count > 0)
                            firstVertice = pair.Item1.Triangulate(_triangles);
                        if (pair.Item2.Path.Count > 0)
                            secondVertice = pair.Item2.Triangulate(_triangles);
                        if (firstVertice != null && secondVertice != null)
                            firstVertice.ConnectWithMirror(secondVertice);
                    }
                }
            }

            #endregion // End of Apply edge constrains

            #region Rearrange data (II)

            foreach (Vertex point in _points)
            {
                point.X += xdis;
                point.Y += ydis;
            }

            #endregion // End of Rearrange data (II)
        }

        Triangle TriangleIncluding(Vertex point)
        {
            Edge vertice = _rightExtremeTriangle.Edges[0];

            do
            {
                Triangle triangle = vertice.IncludingTriangle;
                if (triangle.VerticesIncluded.Contains<Vertex>(point))
                    return triangle;
                vertice = vertice.NextIn.MirrorOut;
            }
            while (vertice != null);

            return null;
        }

        void CreateLocalTriangulationsBoundary(List<Tuple<Edge, TriangleBranch>> boundary,
            TriangleBranch previousBranch, Edge vertice, Vertex point)
        {
            if (vertice.MirrorOut != null)
            {
                VertexCircleRelation relation = vertice.MirrorOut.NextIn.EndVertex.
                    IsRelativeToCircle(vertice.EndVertex, point, vertice.StartVertex);

                if (relation == VertexCircleRelation.Outside ||
                    relation == VertexCircleRelation.Cocircular)
                    boundary.Add(new Tuple<Edge, TriangleBranch>(vertice, previousBranch));
                else
                {
                    var branch = new TriangleBranch(vertice.MirrorOut.IncludingTriangle, previousBranch);
                    CreateLocalTriangulationsBoundary(boundary, branch, vertice.MirrorOut.NextIn, point);
                    CreateLocalTriangulationsBoundary(boundary, branch, vertice.MirrorOut.NextIn.NextIn, point);
                    _triangles.Remove(vertice.MirrorOut.IncludingTriangle);
                }
            }
            else boundary.Add(new Tuple<Edge, TriangleBranch>(vertice, previousBranch));
        }

        Tuple<Edge, Edge> AddSlice(Tuple<Edge, TriangleBranch> bound, Vertex point)
        {
            Edge vertice = bound.Item1;
            TriangleBranch branch = bound.Item2;

            var slice = new Triangle(vertice, point, 0, true);
            do
            {
                int j = 0;
                while (j < branch.Node.VerticesIncluded.Count)
                {
                    Vertex pointToInclude = branch.Node.VerticesIncluded[j];
                    VertexVectorOrientation o1 = pointToInclude.IsRelativeToLine(vertice.NextIn),
                                            o2 = pointToInclude.IsRelativeToLine(vertice.NextIn.NextIn);

                    if ((o2 == VertexVectorOrientation.ToTheLeftOf ||
                         o2 == VertexVectorOrientation.Colinear) &&
                         o1 == VertexVectorOrientation.ToTheLeftOf)
                    {
                        slice.VerticesIncluded.Add(pointToInclude);
                        branch.Node.VerticesIncluded.Remove(pointToInclude);
                    }
                    else j++;
                }
                branch = branch.PreviousNode;
            }
            while (branch != null);

            _triangles.Add(slice);
            if (vertice.StartVertex == _m && vertice.EndVertex == _k)
                _rightExtremeTriangle = slice;

            return new Tuple<Edge, Edge>(slice.Edges[2], slice.Edges[1]);
        }

        EdgesRing GetTriangulationsClosedBoundary()
        {
            var boundary = new EdgesRing();
            Edge vertice = _rightExtremeTriangle.Edges[1];

            for (int i = 0; i < 3; i++)
            {
                _triangles.Remove(vertice.IncludingTriangle);
                while (vertice.MirrorOut.NextIn.MirrorOut != null)
                {
                    vertice = vertice.MirrorOut.NextIn;
                    boundary.AddDataToEnd(vertice.NextIn);
                }
                vertice = vertice.MirrorOut.NextIn.NextIn;
            }
            return boundary;
        }

        #region Constrain application methods

        List<Tuple<EdgesPath, EdgesPath>> GetSequences(Vertex a, Vertex b)
        {
            return GetSequences(a, b, GetPointBoundary(a));
        }

        List<Tuple<EdgesPath, EdgesPath>> GetSequences(Vertex a, Vertex b, Edge vertice)
        {
            return GetSequences(a, b, GetStartingPointBoundary(vertice));
        }

        List<Tuple<EdgesPath, EdgesPath>> GetSequences(Vertex a, Vertex b, List<Edge> vertices)
        {
            if (a.IsEqualToByRef(b)) return null;

            var ab = new Vector(a, b);
            Edge vertice = vertices[0];
            if (vertice.StartVertex.IsRelativeToLine(ab) == VertexVectorOrientation.Colinear)
                return GetSequences(vertice.StartVertex, b, vertice);

            int i = 0;
            while (i < vertices.Count)
            {
                vertice = vertices[i];
                VertexVectorOrientation o = vertice.EndVertex.IsRelativeToLine(ab);
                if (o == VertexVectorOrientation.Colinear)
                    return GetSequences(vertice.EndVertex, b, vertice.NextIn);
                if (b.IsRelativeToLine(vertice) == VertexVectorOrientation.ToTheRightOf &&
                    vertice.StartVertex.IsRelativeToLine(ab) == VertexVectorOrientation.ToTheRightOf &&
                    o == VertexVectorOrientation.ToTheLeftOf)
                {
                    var segments = new List<Tuple<EdgesPath, EdgesPath>>();
                    List<Vector> leftPath = new List<Vector>(),
                                 rightPath = new List<Vector>();
                    leftPath.Add(vertice.NextIn);
                    rightPath.Add(vertice.NextIn.NextIn);
                    vertice = vertice.MirrorOut.NextIn;
                    bool stop = false;
                    while (!(vertice.EndVertex.IsEqualToByRef(b) || stop))
                    {
                        switch (vertice.EndVertex.IsRelativeToLine(ab))
                        {
                            case VertexVectorOrientation.Colinear:
                                segments.AddRange(GetSequences(vertice.EndVertex, b, vertice.NextIn));
                                stop = true;
                                break;
                            case VertexVectorOrientation.ToTheLeftOf:
                                leftPath.Add(vertice.NextIn);
                                vertice = vertice.MirrorOut.NextIn;
                                break;
                            case VertexVectorOrientation.ToTheRightOf:
                                rightPath.Insert(0, vertice);
                                vertice = vertice.NextIn.MirrorOut.NextIn;
                                break;
                        }
                    }
                    leftPath.Add(vertice.NextIn);
                    rightPath.Insert(0, vertice);
                    segments.Add(new Tuple<EdgesPath, EdgesPath>
                        (new EdgesPath(leftPath), new EdgesPath(rightPath)));

                    return segments;
                }
                i++;
            }
            return null;
        }

        List<Edge> GetPointBoundary(Vertex point)
        {
            return GetStartingPointBoundary(FindFirstVerticeStartingAt(point));
        }

        static List<Edge> GetStartingPointBoundary(Edge vertice)
        {
            var vertices = new List<Edge>();
            Edge firstVertice = vertice;
            vertice = vertice.NextIn;
            vertices.Add(vertice);
            vertice = vertice.NextIn;
            while (vertice.MirrorOut != null &&
                   vertice.MirrorOut != firstVertice)
            {
                vertice = vertice.MirrorOut.NextIn;
                vertices.Add(vertice);
                vertice = vertice.NextIn;
            }
            return vertices;
        }

        Edge FindFirstVerticeStartingAt(Vertex point)
        {
            return _triangles.FindVertice<Triangle, Edge, Vertex>(v => v.StartVertex.IsEqualToByRef(point));
        }

        #endregion // End of Constrain application methods

        class YdimComparer :
            IComparer<Vertex>
        {
            public int Compare(Vertex pA, Vertex pB)
            {
                return pA.Y.CompareTo(pB.Y);
            }
        }

        class PointComparer :
            IEqualityComparer<Vertex>
        {
            public bool Equals(Vertex a, Vertex b)
            {
                return ReferenceEquals(a, b);
                /*if (Object.ReferenceEquals(A, B)) return true;
                if (Object.ReferenceEquals(A, null) || Object.ReferenceEquals(B, null)) return false;
                return A.IsEqualTo(B);*/
            }

            public int GetHashCode(Vertex point)
            {
                if (ReferenceEquals(point, null)) return 0;
                return point.X.GetHashCode() ^ point.Y.GetHashCode();
            }
        }
    }

    public class TriangulatorTicketIn
    {
        [Channel]
        public readonly List<Vertex> Points;
        [Channel]
        public readonly List<Tuple<Vertex, Vertex>> Constrains;

        public TriangulatorTicketIn(List<Vertex> points, List<Tuple<Vertex, Vertex>> constrains)
        {
            Points = points;
            Constrains = constrains;
        }
    }

    public class TriangulatorTicketOut
    {
        [Channel]
        public readonly List<Triangle> Triangles;

        public TriangulatorTicketOut(List<Triangle> triangles)
        {
            Triangles = triangles;
        }
    }
}
