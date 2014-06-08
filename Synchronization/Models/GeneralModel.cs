using System;
using System.Collections.Generic;
using Synchronization.Core;

namespace Synchronization.Models
{
    /// <summary>
    /// Provides access to the input of a process.
    /// </summary>
    /// <typeparam name="TTicket">the input ticket type</typeparam>
    public interface IFeedable<in TTicket> :
        ISynchronizable
        where TTicket : class
    {
        void EnQueue(TTicket ticket);
    }

    /// <summary>
    /// Fiber base model of worker thread implementing looping models.
    /// </summary>
    /// <typeparam name="TUnit">processing unit</typeparam>
    /// <typeparam name="TParameters">thread local parameters</typeparam>
    /// <typeparam name="TTicketIn">job ticket type</typeparam>
    /// <typeparam name="TTicketOut">result ticket type</typeparam>
    public abstract class LoopingFiber<TUnit, TParameters, TTicketIn, TTicketOut> :
        ParameterizedFiber<TParameters>
        where TUnit : class, IWorker<TTicketIn, TTicketOut>, new()
        where TParameters : class, new()
        where TTicketIn : class
        where TTicketOut : class
    {
        #region Initialization

        protected LoopingFiber() {}

        protected LoopingFiber(TParameters parameters)
            : base(parameters) {}

        #endregion // End of Initialization

        /// <summary>
        /// The processing unit.
        /// </summary>
        readonly TUnit _unit = new TUnit();

        /// <summary>
        /// The method that processes the tickets.
        /// </summary>
        /// <param name="ticket">the ticket</param>
        /// <returns>the result of the calculation</returns>
        protected TTicketOut DoWork(TTicketIn ticket)
        {
            return _unit.DoWork(ticket);
        }
    }

    /// <summary>
    /// The interface of the loop workers.
    /// </summary>
    /// <typeparam name="TTicketIn">job ticket type</typeparam>
    /// <typeparam name="TTicketOut">result ticket type</typeparam>
    public interface IWorker<in TTicketIn, out TTicketOut>
        where TTicketIn : class
        where TTicketOut : class
    {
        TTicketOut DoWork(TTicketIn ticket);
    }

    /// <summary>
    /// Input and Output queues.
    /// </summary>
    /// <typeparam name="TTicketIn">Job ticket type.</typeparam>
    /// <typeparam name="TTicketOut">Result ticket type.</typeparam>
    public sealed class InOutQueues<TTicketIn, TTicketOut>
        where TTicketIn : class
        where TTicketOut : class
    {
        public TsQueue<TTicketIn> Input { get; set; }
        public TsQueue<TTicketOut> Output { get; set; }

        public InOutQueues(TsQueue<TTicketIn> queueIn, TsQueue<TTicketOut> queueOut)
        {
            Input = queueIn;
            Output = queueOut;
        }

        public void EnQueue(TTicketIn job)
        {
            if (Input != null) Input.Enqueue(job);
        }

        public bool DequeueIfAny(out TTicketOut result)
        {
            if (Output != null)
                return Output.DequeueIfAny(out result);
            result = null;
            return false;
        }
    }

    /// <summary>
    /// Thread-safe version of System.Collections.Generic.Queue.
    /// </summary>
    /// <typeparam name="T">Queued objects type.</typeparam>
    public sealed class TsQueue<T>
    {
        readonly Queue<T> _queue;
        readonly object _queueLock;
        IWakeable _node;

        public TsQueue()
        {
            _queueLock = new object();
            _queue = new Queue<T>();
        }

        public TsQueue(IWakeable node) : this()
        {
            _node = node;
        }

        /// <summary>
        /// Thread-safe version of Queue.Enqueue(T).
        /// </summary>
        /// <param name="element">Object to enqueue.</param>
        public void Enqueue(T element)
        {
            bool wakeup = false;

            lock (_queueLock)
            {
                _queue.Enqueue(element);
                if (_queue.Count == 1) wakeup = true;
            }
            if (wakeup && _node != null) _node.WakeUp();
        }

        /// <summary>
        /// Thread-safe version of Queue.Dequeue().
        /// </summary>
        /// <returns>Next in queued object.</returns>
        public T Dequeue()
        {
            lock (_queueLock) return _queue.Dequeue();
        }

        /// <summary>
        /// Thread-safe version of Queue.Count.
        /// </summary>
        public int Count
        {
            get { lock (_queueLock) return _queue.Count; }
        }

        /// <summary>
        /// Dequeues the next element in the queue if it exists.
        /// </summary>
        /// <param name="job">Next in queued object.</param>
        /// <returns>False if the queue was empty.</returns>
        public bool DequeueIfAny(out T job)
        {
            lock (_queueLock) if (_queue.Count > 0)
            {
                job = _queue.Dequeue();
                return true;
            }
            job = default(T);
            return false;
        }

        /// <summary>
        /// Dequeues all elements in the queue if they exist.
        /// </summary>
        /// <param name="jobs"></param>
        /// <returns></returns>
        public bool DequeueAllIfAny(List<T> jobs)
        {
            lock (_queueLock) if (_queue.Count > 0)
            {
                while (_queue.Count > 0)
                    jobs.Add(_queue.Dequeue());
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the fiber array that the queue feeds.
        /// </summary>
        /// <param name="node">The fiber array.</param>
        public void SetNode(IWakeable node)
        {
            _node = node;
        }
    }

    /// <summary>
    /// Ticket wrapper
    /// </summary>
    /// <typeparam name="TTicket">ticket type</typeparam>
    public class JobTicket<TTicket>
        where TTicket : class
    {
        public long Id { get; private set; }
        public TTicket Ticket { get; set; }

        public JobTicket(long id)
        {
            Id = id;
        }
    }

    /// <summary>
    /// Defines a field of a ticket as a splittable parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ChannelAttribute : Attribute { }
}
