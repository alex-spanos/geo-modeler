using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Geometry
{
    public class SimpleFormatter<TTriangle, TEdge, TVertex> :
        IGeometryFormatter<TTriangle, TEdge, TVertex>
        where TTriangle : TrianglePrototype<TTriangle, TEdge, TVertex, double>, new()
        where TEdge : VectorPrototype<TVertex, double>, IEdgePrototype<TTriangle, TEdge, TVertex, double>, new()
        where TVertex : VertexPrototype<double>, new()
    {
        CultureInfo _ci;

        public SimpleFormatter(string culture)
        {
            SetCulture(culture);
        }

        public void SetCulture(string culture)
        {
            _ci = CultureInfo.GetCultureInfo(culture);
        }

        public List<TTriangle> ReadTriangles(string[] inputLines, out List<TVertex> points, int start, int end)
        {
            int dif;

            points = null;
            if (ErrorInput(inputLines, start, end, out dif)) return null;
            points = new List<TVertex>();
            var triangles = new List<TTriangle>(dif);

            for (int i = start; i <= end; i++)
            {
                if (string.IsNullOrWhiteSpace(inputLines[i])) continue;
                string[] coords = inputLines[i].Split(new[] { ' ' });
                if (coords.Length < 9) continue;
                TVertex vertexA = points.GetPoint(ReadPoint(coords[0], coords[1], coords[2]), true);
                if (vertexA == null) continue;
                TVertex vertexB = points.GetPoint(ReadPoint(coords[3], coords[4], coords[5]), true);
                if (vertexB == null) continue;
                TVertex vertexC = points.GetPoint(ReadPoint(coords[6], coords[7], coords[8]), true);
                if (vertexC == null) continue;
                var triangle = new TTriangle();
                triangle.Init(vertexA, vertexB, vertexC);
                triangles.Add(triangle);
            }
            return triangles;
        }

        public string[] WriteTriangles(List<TTriangle> triangles)
        {
            if (triangles == null || triangles.Count == 0) return null;
            var trianglesLines = new List<string>(triangles.Count);

            trianglesLines.AddRange(from t in triangles
                                    where t != null
                                    select WritePoint(t.Edges[0].EndVertex) + " " +
                                           WritePoint(t.Edges[1].EndVertex) + " " +
                                           WritePoint(t.Edges[2].EndVertex));

            return trianglesLines.ToArray();
        }

        public List<Tuple<TVertex, TVertex>> ReadConstrains(List<TVertex> points, string[] inputLines, int start, int end)
        {
            int dif;

            if (ErrorInput(inputLines, start, end, out dif)) return null;
            var constrains = new List<Tuple<TVertex, TVertex>>(dif);

            for (int i = start; i <= end; i++)
            {
                if (string.IsNullOrWhiteSpace(inputLines[i])) continue;
                string[] coords = inputLines[i].Split(new[] { ' ' });
                if (coords.Length < 6) continue;
                TVertex vertexA = points.GetPoint(ReadPoint(coords[0], coords[1], coords[2]), true);
                if (vertexA == null) continue;
                TVertex vertexB = points.GetPoint(ReadPoint(coords[3], coords[4], coords[5]), true);
                if (vertexB == null) continue;
                constrains.Add(new Tuple<TVertex, TVertex>(vertexA, vertexB));
            }
            return constrains;
        }

        public string[] WriteConstrains(List<Tuple<TVertex, TVertex>> constrains)
        {
            if (constrains == null || constrains.Count == 0) return null;
            var constrainsLines = new List<string>(constrains.Count);

            constrainsLines.AddRange(from t in constrains
                                     where t != null && t.Item1 != null && t.Item2 != null
                                     select WritePoint(t.Item1) + " " + WritePoint(t.Item2));

            return constrainsLines.ToArray();
        }

        public List<Tuple<TVertex, TVertex>> ReadIsoLines(string[] inputLines, out List<TVertex> isoPoints, int start, int end)
        {
            int dif;

            isoPoints = null;
            if (ErrorInput(inputLines, start, end, out dif)) return null;
            isoPoints = new List<TVertex>();
            var isoLines = new List<Tuple<TVertex, TVertex>>(dif);

            for (int i = start; i <= end; i++)
            {
                if (string.IsNullOrWhiteSpace(inputLines[i])) continue;
                string[] coords = inputLines[i].Split(new[] { ' ' });
                if (coords.Length < 6) continue;
                TVertex isoPointA = isoPoints.GetPoint(ReadPoint(coords[0], coords[1], coords[2]), true);
                if (isoPointA == null) continue;
                TVertex isoPointB = isoPoints.GetPoint(ReadPoint(coords[3], coords[4], coords[5]), true);
                if (isoPointB == null) continue;
                var isoline = new Tuple<TVertex, TVertex>(isoPointA, isoPointB);
                if (!isoLines.PairExists(isoline)) isoLines.Add(isoline);
            }
            return isoLines;
        }

        public string[] WriteIsoLines(List<List<Tuple<TVertex, TVertex>>> isoLines)
        {
            if (isoLines == null || isoLines.Count == 0) return null;
            var outputLines = new List<string>(isoLines.Count);

            foreach (List<Tuple<TVertex, TVertex>> isoLine in isoLines.Where(isoLine => isoLine != null))
                outputLines.AddRange(from t in isoLine
                                     where t != null && t.Item1 != null && t.Item2 != null
                                     select WritePoint(t.Item1) + " " + WritePoint(t.Item2));

            return outputLines.ToArray();
        }

        public List<TVertex> ReadPoints(string[] inputLines, int start, int end)
        {
            int dif;

            if (ErrorInput(inputLines, start, end, out dif)) return null;
            var points = new List<TVertex>(dif);

            for (int i = start; i <= end; i++)
            {
                if (string.IsNullOrWhiteSpace(inputLines[i])) continue;
                string[] coords = inputLines[i].Split(new[] { ' ' });
                if (coords.Length < 3) continue;
                points.GetPoint(ReadPoint(coords[0], coords[1], coords[2]), true);
            }
            return points;
        }

        public string[] WritePoints(List<TVertex> points)
        {
            if (points == null || points.Count == 0) return null;
            var pointLines = new List<string>(points.Count);

            pointLines.AddRange(from t in points
                                where t != null
                                select WritePoint(t));

            return pointLines.ToArray();
        }

        public bool ParseDouble(string text, out double result)
        {
            return double.TryParse(text, NumberStyles.Float, _ci, out result);
        }

        TVertex ReadPoint(string inputText)
        {
            return ReadPoint(inputText.Split(new[] { ' ' }));
        }

        TVertex ReadPoint(string[] coords)
        {
            return coords.Length == 3 ? ReadPoint(coords[0], coords[1], coords[2]) : null;
        }

        private TVertex ReadPoint(string coordX, string coordY, string coordZ)
        {
            double x, y, z;

            return double.TryParse(coordX, NumberStyles.Float, _ci, out x) &&
                   double.TryParse(coordY, NumberStyles.Float, _ci, out y) &&
                   double.TryParse(coordZ, NumberStyles.Float, _ci, out z)
                       ? new TVertex
                       {
                           X = x,
                           Y = y,
                           Z = z
                       }
                       : null;
        }

        string WritePoint(TVertex point)
        {
            if (point == null) return String.Empty;
            return point.X.ToString(_ci.NumberFormat) + " " +
                   point.Y.ToString(_ci.NumberFormat) + " " +
                   point.Z.ToString(_ci.NumberFormat);
        }

        static bool ErrorInput(string[] lines, int start, int end, out int dif)
        {
            dif = end - start + 1;
            return lines == null || end < 0 || start < 0 || dif < 1 ||
                   lines.Length < start || lines.Length < end;
        }
    }

    public interface IGeometryFormatter<TTriangle, TEdge, TVertex>
        where TTriangle : TrianglePrototype<TTriangle, TEdge, TVertex, double>, new()
        where TEdge : VectorPrototype<TVertex, double>, IEdgePrototype<TTriangle, TEdge, TVertex, double>, new()
        where TVertex : VertexPrototype<double>, new()
    {
        List<TTriangle> ReadTriangles(string[] inputLines, out List<TVertex> points, int start, int end);

        string[] WriteTriangles(List<TTriangle> triangles);

        List<Tuple<TVertex, TVertex>> ReadConstrains(List<TVertex> points, string[] inputLines, int start, int end);

        string[] WriteConstrains(List<Tuple<TVertex, TVertex>> constrains);

        List<Tuple<TVertex, TVertex>> ReadIsoLines(string[] inputLines, out List<TVertex> isoPoints, int start, int end);

        string[] WriteIsoLines(List<List<Tuple<TVertex, TVertex>>> isoLines);

        List<TVertex> ReadPoints(string[] inputLines, int start, int end);

        string[] WritePoints(List<TVertex> points);

        bool ParseDouble(string text, out double result);
    }
}
