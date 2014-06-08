using Synchronization.Core;

namespace Synchronization.Models.ScatterGather
{
    /// <summary>
    /// Array of scatter-gather model fibers.
    /// </summary>
    /// <typeparam name="TWorker">scatter-Gather fiber</typeparam>
    /// <typeparam name="TUnit">processing unit</typeparam>
    /// <typeparam name="TParameters">scatter-Gather thread local fiber parameters</typeparam>
    /// <typeparam name="TTicketIn">job ticket type</typeparam>
    /// <typeparam name="TTicketOut">result ticket type</typeparam>
    public class ScatterGatherFiberArray<TWorker, TUnit, TParameters, TTicketIn, TTicketOut> :
        ParameterizedFiberArray<TWorker, TParameters>
        where TWorker : ScatterGatherFiber<TUnit, TParameters, TTicketIn, TTicketOut>, new()
        where TUnit : class, IWorker<TTicketIn, TTicketOut>, new()
        where TParameters : ScatterGatherFiberParameters<TTicketIn, TTicketOut>, new()
        where TTicketIn : class
        where TTicketOut : class
    {
        #region Initialization

        public ScatterGatherFiberArray(int noW, TParameters parameters)
            : base(noW, parameters)
        {
            Parameters.Queues.Input.SetNode(this);
        }

        public ScatterGatherFiberArray(int noW, InOutQueues<JobTicket<TTicketIn>, JobTicket<TTicketOut>> queues)
            : this(noW, new TParameters {Queues = queues}) {}

        #endregion // End of Initialization
    }

    /// <summary>
    /// Fiber base model of worker thread implemented for the scatter-gather model.
    /// </summary>
    /// <typeparam name="TUnit">processing unit</typeparam>
    /// <typeparam name="TParameters">thread local parameters</typeparam>
    /// <typeparam name="TTicketIn">job ticket type</typeparam>
    /// <typeparam name="TTicketOut">result ticket type</typeparam>
    public class ScatterGatherFiber<TUnit, TParameters, TTicketIn, TTicketOut> :
        LoopingFiber<TUnit, TParameters, TTicketIn, TTicketOut>
        where TUnit : class, IWorker<TTicketIn, TTicketOut>, new()
        where TParameters : ScatterGatherFiberParameters<TTicketIn, TTicketOut>, new()
        where TTicketIn : class
        where TTicketOut : class
    {
        #region Initialization

        public ScatterGatherFiber() {}

        public ScatterGatherFiber(TParameters parameters)
            : base(parameters)
        {
            Parameters.Queues.Input.SetNode(this);
        }

        public ScatterGatherFiber(InOutQueues<JobTicket<TTicketIn>, JobTicket<TTicketOut>> queues)
            : this(new TParameters {Queues = queues}) {}

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
                JobTicket<TTicketIn> ticketIn;
                if (Parameters.Queues.Input.DequeueIfAny(out ticketIn))
                    Parameters.Queues.Output.Enqueue(new JobTicket<TTicketOut>(ticketIn.Id)
                                                         {Ticket = DoWork(ticketIn.Ticket)});
                else PauseRequested = true;
            }
            Exited = true;
        }
    }

    /// <summary>
    /// Fiber base parameters for the scatter-gather model.
    /// </summary>
    /// <typeparam name="TTicketIn">Job ticket type</typeparam>
    /// <typeparam name="TTicketOut">Result ticket type</typeparam>
    public class ScatterGatherFiberParameters<TTicketIn, TTicketOut>
        where TTicketIn : class
        where TTicketOut : class
    {
        public InOutQueues<JobTicket<TTicketIn>, JobTicket<TTicketOut>> Queues { get; set; }

        #region Initialization

        public ScatterGatherFiberParameters() {}

        public ScatterGatherFiberParameters(TsQueue<JobTicket<TTicketIn>> queueIn, TsQueue<JobTicket<TTicketOut>> queueOut)
        {
            Queues = new InOutQueues<JobTicket<TTicketIn>, JobTicket<TTicketOut>>(queueIn, queueOut);
        }

        public ScatterGatherFiberParameters(InOutQueues<JobTicket<TTicketIn>, JobTicket<TTicketOut>> queues)
        {
            Queues = queues;
        }

        #endregion // End of Initialization
    }
}
