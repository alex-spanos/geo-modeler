using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using BoxBridge;
//using Geometry;
using Synchronization.Models;
using Synchronization.Models.ScatterGather;
//using Topography.Algorithms;
//using Topography.DataStructures;

namespace BoxBuilder
{
    /// <summary>
    /// Calculation box.
    /// </summary>
    public class Box
    {
        #region Fiber arrays description

        /// <summary>
        /// Scatter-Gather fiber arrays description.
        /// </summary>
        readonly static Type[][] FiberArraysDescription = {
            //new[] {
            //    typeof(ScatterGatherFiber<Triangulator, ScatterGatherFiberParameters<TriangulatorTicketIn, TriangulatorTicketOut>, TriangulatorTicketIn, TriangulatorTicketOut>),
            //    typeof(Triangulator),
            //    typeof(ScatterGatherFiberParameters<TriangulatorTicketIn, TriangulatorTicketOut>),
            //    typeof(JobTicket<TriangulatorTicketIn>),
            //    typeof(JobTicket<TriangulatorTicketOut>) },
            //new[] {
            //    typeof(ScatterGatherFiber<ContourMapper, ScatterGatherFiberParameters<ContourMapperTicketIn, ContourMapperTicketOut>, ContourMapperTicketIn, ContourMapperTicketOut>),
            //    typeof(ContourMapper),
            //    typeof(ScatterGatherFiberParameters<ContourMapperTicketIn, ContourMapperTicketOut>),
            //    typeof(JobTicket<ContourMapperTicketIn>),
            //    typeof(JobTicket<ContourMapperTicketOut>) }
        };

        /// <summary>
        /// The maximum number of stages supported.
        /// </summary>
        public readonly static int MaxStage = FiberArraysDescription.GetLength(0);

        #endregion // End of Fiber arrays description

        ScatterGatherFiberBlock Block { get; set; }
        /*
        public int SingleCalculation(int startingStage, int endingStage, int[] numbersOfWorkers, string inputFileName, string outputFileName)
        {
            string[] inputFileLines, outputFileLines = null;

            try { inputFileLines = File.ReadAllLines(inputFileName); }
            catch (Exception) { return (int)BoxError.ReadingInputFile; }

            WaitForClose();
            Block = CreateInstanse(startingStage, endingStage, numbersOfWorkers);
            if (Block == null) return (int)BoxError.BlockInitialization;

            List<Vertex> points;
            switch (startingStage)
            {
                case 1:
                    List<Tuple<Vertex, Vertex>> constrains;
                    ParseTriangulatorLines(inputFileLines, out points, out constrains);
                    Block.Queues.EnQueue(new TriangulatorTicketIn(points, constrains));
                    break;
                case 2:
                    List<Triangle> triangles;
                    double isoDimention;
                    ParseContourMapperLines(inputFileLines, out points, out triangles, out isoDimention);
                    Block.Queues.EnQueue(new ContourMapperTicketIn(points, triangles, isoDimention));
                    break;
            }
            if (Block.Start() != 0) return (int)BoxError.BlockInitialization;

            switch (endingStage)
            {
                case 1:
                    TriangulatorTicketOut triangulatorResult;
                    while (!Block.Queues.DequeueIfAny(out triangulatorResult)) Thread.Sleep(100);
                    outputFileLines = Formater.WriteTriangles(triangulatorResult.Triangles);
                    break;
                case 2:
                    ContourMapperTicketOut contourMapperResult;
                    while (!Block.Queues.DequeueIfAny(out contourMapperResult)) Thread.Sleep(100);
                    outputFileLines = Formater.WriteIsoLines(contourMapperResult.IsoLines);
                    break;
            }
            WaitForClose();

            if (outputFileLines != null)
                try { File.WriteAllLines(outputFileName, outputFileLines); }
                catch (Exception) { return (int)BoxError.WritingOutputFile; }

            return (int)BoxError.NoError;
        }
        */
        public int SetupForService(int startingStage, int endingStage, int[] numbersOfWorkers)
        {
            WaitForClose();
            Block = CreateInstanse(startingStage, endingStage, numbersOfWorkers);
            return Block.Start();
        }

        public void EnQueueTicket(BoxTicketIn ticketIn)
        {
            //Block.Queues.EnQueue();
        }

        public void PauseProcessing()
        {
            if (Block != null) Block.PauseRequested = true;
        }

        public void ResumeProcessing()
        {
            if (Block != null) Block.PauseRequested = false;
        }

        public void WaitForClose()
        {
            if (Block == null) return;
            Block.Stop();
            Block.Close();
        }

        /// <summary>
        /// Creates an instance of the scatter-gather fiber block.
        /// </summary>
        /// <param name="startingStage">The starting stage.</param>
        /// <param name="endingStage">The ending stage.</param>
        /// <param name="numbersOfWorkers">The number of workers per stage.</param>
        /// <returns>The fiber block instance.</returns>
        static ScatterGatherFiberBlock CreateInstanse(int startingStage, int endingStage, int[] numbersOfWorkers)
        {
            startingStage--;
            int dif = endingStage - startingStage;

            if (numbersOfWorkers.Length != dif ||
                endingStage > FiberArraysDescription.GetLength(0)) return null;

            var fad = new object[dif * 2];

            for (int i = 0; i < dif; i++)
            {
                int k = 2 * i;
                fad[k] = FiberArraysDescription[i + startingStage];
                fad[k + 1] = numbersOfWorkers[i];
            }

            return new ScatterGatherFiberBlock(fad);
        }
        /*
        static void ParseTriangulatorLines(string[] inputLines, out List<Vertex> points, out List<Tuple<Vertex, Vertex>> constrains)
        {
            points = null;
            constrains = null;
            bool cut;
            if (inputLines == null || inputLines.Length == 0) return;
            int startPoints = GetNextLine(inputLines, 0, false, out cut);
            if (!cut) return;
            int endPoints = GetNextLine(inputLines, startPoints + 1, true, out cut) - 1;
            points = Formater.ReadPoints(inputLines, startPoints, endPoints);
            if (!cut) return;
            int startConstrains = GetNextLine(inputLines, endPoints + 2, false, out cut);
            if (!cut) return;
            int endConstrains = GetNextLine(inputLines, startConstrains, true, out cut) - 1;
            constrains = Formater.ReadConstrains(points, inputLines, startConstrains, endConstrains);
        }

        static void ParseContourMapperLines(string[] inputLines, out List<Vertex> points, out List<Triangle> triangles, out double isoDimention)
        {
            points = null;
            triangles = null;
            isoDimention = 0;
            bool cut;
            if (inputLines == null || inputLines.Length == 0) return;
            int start = GetNextLine(inputLines, 0, false, out cut);
            if (!cut) return;
            if (Formater.ParseDouble(inputLines[start], out isoDimention)) return;
            start = GetNextLine(inputLines, start + 1, false, out cut);
            if (!cut) return;
            int end = GetNextLine(inputLines, start + 1, true, out cut) - 1;
            triangles = Formater.ReadTriangles(inputLines, out points, start, end);
        }
        */
        /// <summary>
        /// Parses a string array to find the first empty or non empty line.
        /// </summary>
        /// <param name="lines">The string array to be parsed.</param>
        /// <param name="start">The position at which the parsing begins.</param>
        /// <param name="empty">True to find next empty line, false to find the next non empty line..</param>
        /// <param name="found">True if the search has found the requested line, false if not.</param>
        /// <returns>The position at which the search stoped (length plus one if search was unsuccessfull).</returns>
        static int GetNextLine(string[] lines, int start, bool empty, out bool found)
        {
            int i = start;
            found = false;
            while (i < lines.Length && !found)
                if (empty ? lines[i] == "" : lines[i] != "") found = true;
                else i++;
            return i;
        }

        //// <summary>
        //// Standart formatter.
        //// </summary>
        //static readonly SimpleFormatter<Triangle, Edge, Vertex> Formater = new SimpleFormatter<Triangle, Edge, Vertex>("en-US");
    }

    public enum BoxError
    {
        NoError = 0,
        BlockInitialization = 300,
        ReadingInputFile = 301,
        WritingOutputFile = 302
    }
}
