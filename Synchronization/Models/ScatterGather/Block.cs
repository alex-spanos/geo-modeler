using System;
using System.Collections.Generic;
using System.Reflection;
using Synchronization.Core;

namespace Synchronization.Models.ScatterGather
{
    /// <summary>
    /// Sequense of various types of fiber arrays implementing
    /// the scatter-gather model of parallel processing.
    /// </summary>
    public class ScatterGatherFiberBlock :
        FiberArray<FiberArray<ISynchronizable>>
    {
        #region Parameterization

        /// <summary>
        /// Scatter-gather fiber parameters types.
        /// </summary>
        protected List<Type> ScatterGatherFiberParametersTypes;

        /// <summary>
        /// Scatter-gather fiber array ticket types.
        /// </summary>
        protected List<Type> ScatterGatherFiberTicketTypes;

        /// <summary>
        /// Thread-safe queues types.
        /// </summary>
        protected List<Type> TsQueuesTypes;

        /// <summary>
        /// Thread-safe queues.
        /// </summary>
        protected List<object> TsQueues;

        #endregion // End of Parameterization

        /// <summary>
        /// The input and output pair of queues of the scatter-gather fiber block.
        /// </summary>
        public dynamic Queues;

        /// <summary>
        /// Initializes a new fiber-block.
        /// </summary>
        /// <param name="fiberArraysDescription">array of objects of consecutive tuples 
        /// of Type[4] and int describing the types of the Scatter-Gather model fiber-arrays</param>
        public ScatterGatherFiberBlock(params object[] fiberArraysDescription)
            : base(fiberArraysDescription == null
                       ? 0 : fiberArraysDescription.Length%2 != 0
                             ? 0 : fiberArraysDescription.Length/2)
        {
            bool error = false;

            if (NumberOfWorkers == 0) return;
            ScatterGatherFiberParametersTypes = new List<Type>(NumberOfWorkers);
            ScatterGatherFiberTicketTypes = new List<Type>(NumberOfWorkers + 1);
            TsQueuesTypes = new List<Type>(NumberOfWorkers + 1);
            TsQueues = new List<object>(NumberOfWorkers + 1);

            #region Append arrays

            for (int i = 0; i < NumberOfWorkers; i = i + 2)
            {
                Type[] description;
                try
                {
                    description = (Type[]) fiberArraysDescription[i];
                }
                catch (Exception)
                {
                    error = true;
                    break;
                }
                int noW;
                try
                {
                    noW = (int) fiberArraysDescription[i + 1];
                }
                catch (Exception)
                {
                    error = true;
                    break;
                }
                if (description.Length == 5)
                    error = !AppendArray(description[0], description[1], description[2], description[3], description[4], noW);
                else error = true;
                if (error) break;
            }

            #endregion // End of Append arrays

            if (error)
            {
                Workers = null;
                ScatterGatherFiberParametersTypes = null;
                ScatterGatherFiberTicketTypes = null;
                TsQueuesTypes = null;
                TsQueues = null;
                Queues = null;
            }
            else Queues = Rhm.CreateInOutQueuesInstanse(
                Rhm.CreateInOutQueuesType(ScatterGatherFiberTicketTypes[0],
                ScatterGatherFiberTicketTypes[ScatterGatherFiberTicketTypes.Count - 1]),
                TsQueuesTypes[0], TsQueuesTypes[TsQueuesTypes.Count - 1], TsQueues[0], TsQueues[TsQueues.Count - 1]);
        }

        /// <summary>
        /// Creates a new array of scatter-gather worker fibers.
        /// </summary>
        /// <param name="tWorker">scatter-Gather fiber type</param>
        /// <param name="tUnit">processing unit</param>
        /// <param name="tParameters">scatter-Gather fiber parameters type</param>
        /// <param name="tTicketIn">job ticket type type</param>
        /// <param name="tTicketOut">result ticket type type</param>
        /// <param name="rep">number of worker threads int the fiber array</param>
        bool AppendArray(Type tWorker, Type tUnit, Type tParameters, Type tTicketIn, Type tTicketOut, int rep)
        {
            Type scatterGatherFiberBase, scatterGatherFiberParametersBase;

            if (// Check if parameters are non-abstract classes:
                !tWorker.IsClass || tWorker.IsAbstract ||
                !tUnit.IsClass || tUnit.IsAbstract ||
                !tParameters.IsClass || tParameters.IsAbstract ||
                !tTicketIn.IsClass || tTicketIn.IsAbstract ||
                !tTicketOut.IsClass || tTicketOut.IsAbstract ||
                // Check if tWorker is a (closed if generic) subclass of Scatter-Gather Fiber:
                (tWorker.IsGenericType && tWorker.ContainsGenericParameters) ||
                !tWorker.IsSubclassOfRawGeneric(Gtd.ScatterGatherFiberGen, out scatterGatherFiberBase) ||
                // Check if tUnit is a (closed if generic) class and implements IWorker<tTicketIn, tTicketOut>:
                (tUnit.IsGenericType && tUnit.ContainsGenericParameters) ||
                !tUnit.IsWorkerOf(tTicketIn, tTicketOut) ||
                // Check if tParameters is a (closed if generic) subclass of Scatter-Gather Fiber Parameters:
                (tParameters.IsGenericType && tParameters.ContainsGenericParameters) ||
                !tParameters.IsSubclassOfRawGeneric(Gtd.ScatterGatherFiberParametersGen, out scatterGatherFiberParametersBase))
                return false;

            // Check if type constraints hold for tParameters:
            Type[] baseGenericArguments = scatterGatherFiberParametersBase.GetGenericArguments();
            if (!(baseGenericArguments[0] == tTicketIn &&
                  baseGenericArguments[1] == tTicketOut)) return false;

            // Check if type constraints hold for tWorker:
            baseGenericArguments = scatterGatherFiberBase.GetGenericArguments();
            if (!(baseGenericArguments[0] == tUnit &&
                  baseGenericArguments[1] == tParameters &&
                  baseGenericArguments[2] == tTicketIn &&
                  baseGenericArguments[3] == tTicketOut)) return false;

            if (Workers.Count > 0)
            {
                // Check if the fiber array fits in the chain of arrays:
                if (!(ScatterGatherFiberTicketTypes[ScatterGatherFiberTicketTypes.Count - 1] == tTicketIn))
                    return false;
            }
            else
            {
                ScatterGatherFiberTicketTypes.Add(tTicketIn);
                Type inQueueType = Rhm.CreateTsQueueType(tTicketIn);
                object inQueue = Rhm.CreateTsQueueInstanse(inQueueType);
                TsQueuesTypes.Add(inQueueType);
                TsQueues.Add(inQueue);
            }
            Type tOutQueue = Rhm.CreateTsQueueType(tTicketOut);
            object outQueue = Rhm.CreateTsQueueInstanse(tOutQueue);
            Type tInOutQueues = Rhm.CreateInOutQueuesType(ScatterGatherFiberTicketTypes[ScatterGatherFiberTicketTypes.Count - 1], tTicketOut);
            object inOutQueues = Rhm.CreateInOutQueuesInstanse(tInOutQueues, TsQueuesTypes[TsQueuesTypes.Count - 1],
                                                               tOutQueue, TsQueues[TsQueues.Count - 1], outQueue);
            ScatterGatherFiberTicketTypes.Add(tTicketOut);
            TsQueuesTypes.Add(tOutQueue);
            TsQueues.Add(outQueue);
            ScatterGatherFiberParametersTypes.Add(tParameters);

            // Create Scatter-Gather Parameterized Fiber Array type:
            Type tScatterGatherFiberArray = Gtd.ScatterGatherFiberArrayGen.MakeGenericType
                (new[] {tWorker, tUnit, tParameters, tTicketIn, tTicketOut});

            // Create Scatter-Gather Parameterized Fiber Array instance:
            ConstructorInfo fci = tScatterGatherFiberArray.GetConstructor(new[] {typeof (int), tInOutQueues});
            Workers.Add((FiberArray<ISynchronizable>) fci.Invoke(new[] {rep, inOutQueues}));

            return true;
        }

        /// <summary>
        /// Generic type definitions.
        /// </summary>
        private static class Gtd
        {
            /// <summary>
            /// Scatter-Gather Parameterized Fiber Array Generic Type Definition.
            /// </summary>
            public static readonly Type ScatterGatherFiberArrayGen = typeof(ScatterGatherFiberArray<,,,,>);

            /// <summary>
            /// Scatter-Gather Fiber Generic Type Definition.
            /// </summary>
            public static readonly Type ScatterGatherFiberGen = typeof(ScatterGatherFiber<,,,>);

            /// <summary>
            /// Scatter-Gather Fiber Parameters Generic Type Definition.
            /// </summary>
            public static readonly Type ScatterGatherFiberParametersGen = typeof(ScatterGatherFiberParameters<,>);
        }
    }
}
