using System;
using Synchronization.Core;

namespace Synchronization.Models.Workflows
{
    /// <summary>
    /// Array of workflow model fibers.
    /// </summary>
    /// <typeparam name="TWorker">workflow fiber</typeparam>
    /// <typeparam name="TUnit">processing unit</typeparam>
    /// <typeparam name="TParameters">workflow thread local fiber parameters</typeparam>
    /// <typeparam name="TTicketIn">job ticket type</typeparam>
    /// <typeparam name="TTicketOut">result ticket type</typeparam>
    public class FlowFiberArray<TWorker, TUnit, TParameters, TTicketIn, TTicketOut> :
        ParameterizedFiberArray<TWorker, TParameters>,
        IFeedable<Tuple<JobTicket<TTicketIn>, FiberCapsule<TUnit, TTicketIn, TTicketOut>>>
        where TWorker : FlowFiber<TUnit, TParameters, TTicketIn, TTicketOut>, new()
        where TUnit : class, IWorker<TTicketIn, TTicketOut>, new()
        where TParameters : FlowFiberParameters<TUnit, TTicketIn, TTicketOut>, new()
        where TTicketIn : class
        where TTicketOut : class
    {
        #region Initialization

        public FlowFiberArray(int noW)
            : this(noW, new TParameters {Queue = new TsQueue<Tuple<JobTicket<TTicketIn>, FiberCapsule<TUnit, TTicketIn, TTicketOut>>>()}) { }

        public FlowFiberArray(int noW, TParameters parameters)
            : base(noW, parameters)
        {
            Parameters.Queue.SetNode(this);
        }

        public FlowFiberArray(int noW, TsQueue<Tuple<JobTicket<TTicketIn>, FiberCapsule<TUnit, TTicketIn, TTicketOut>>> queue)
            : this(noW, new TParameters {Queue = queue}) {}

        #endregion // End of Initialization

        public void EnQueue(Tuple<JobTicket<TTicketIn>, FiberCapsule<TUnit, TTicketIn, TTicketOut>> ticket)
        {
            Parameters.Queue.Enqueue(ticket);
        }
    }

    /// <summary>
    /// Fiber base model of worker thread implemented for the workflow model.
    /// </summary>
    /// <typeparam name="TUnit">processing unit</typeparam>
    /// <typeparam name="TParameters">thread local parameters</typeparam>
    /// <typeparam name="TTicketIn">job ticket type</typeparam>
    /// <typeparam name="TTicketOut">result ticket type</typeparam>
    public class FlowFiber<TUnit, TParameters, TTicketIn, TTicketOut> :
        LoopingFiber<TUnit, TParameters, TTicketIn, TTicketOut>,
        IFeedable<Tuple<JobTicket<TTicketIn>, FiberCapsule<TUnit, TTicketIn, TTicketOut>>>
        where TUnit : class, IWorker<TTicketIn, TTicketOut>, new()
        where TParameters : FlowFiberParameters<TUnit, TTicketIn, TTicketOut>, new()
        where TTicketIn : class
        where TTicketOut : class
    {
        #region Initialization

        public FlowFiber() {}

        public FlowFiber(TParameters parameters)
            : base(parameters)
        {
            Parameters.Queue.SetNode(this);
        }

        public FlowFiber(TsQueue<Tuple<JobTicket<TTicketIn>, FiberCapsule<TUnit, TTicketIn, TTicketOut>>> queue)
            : this(new TParameters {Queue = queue}) {}

        #endregion // End of Initialization

        /// <summary>
        /// The methods that repeatedly accepts the input tickets and queues out the result.
        /// </summary>
        /// <param name="parameters">Thread local parameters.</param>
        protected override void Worker(object parameters)
        {
            while (!ExitRequested)
            {
                bool hasPaused = PauseIfRequested();
                if (hasPaused && ExitRequested) break;
                Tuple<JobTicket<TTicketIn>, FiberCapsule<TUnit, TTicketIn, TTicketOut>> ticketIn;
                if (Parameters.Queue.DequeueIfAny(out ticketIn))
                    ticketIn.Item2.Forward(new JobTicket<TTicketOut>(ticketIn.Item1.Id)
                                               {Ticket = DoWork(ticketIn.Item1.Ticket)});
                else PauseRequested = true;
            }
            Exited = true;
        }

        public void EnQueue(Tuple<JobTicket<TTicketIn>, FiberCapsule<TUnit, TTicketIn, TTicketOut>> ticket)
        {
            Parameters.Queue.Enqueue(ticket);
        }
    }

    /// <summary>
    /// Fiber base parameters for the workflow model.
    /// </summary>
    /// <typeparam name="TUnit">processing unit</typeparam>
    /// <typeparam name="TTicketIn">Job ticket type</typeparam>
    /// <typeparam name="TTicketOut">Result ticket type</typeparam>
    public class FlowFiberParameters<TUnit, TTicketIn, TTicketOut>
        where TUnit : class, IWorker<TTicketIn, TTicketOut>, new()
        where TTicketIn : class
        where TTicketOut : class
    {
        public TsQueue<Tuple<JobTicket<TTicketIn>, FiberCapsule<TUnit, TTicketIn, TTicketOut>>> Queue { get; set; }

        #region Initialization

        public FlowFiberParameters()
        {
            Queue = new TsQueue<Tuple<JobTicket<TTicketIn>, FiberCapsule<TUnit, TTicketIn, TTicketOut>>>();
        }

        public FlowFiberParameters(TsQueue<Tuple<JobTicket<TTicketIn>, FiberCapsule<TUnit, TTicketIn, TTicketOut>>> queue)
        {
            Queue = queue;
        }

        #endregion // End of Initialization
    }
}
