using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Synchronization.Core
{
    /// <summary>
    /// Array of parameterized fibers.
    /// </summary>
    /// <typeparam name="TWorker">Parameterized fiber type.</typeparam>
    /// <typeparam name="TParameters">Fiber local parameters.</typeparam>
    public class ParameterizedFiberArray<TWorker, TParameters> :
        FiberArray<TWorker>
        where TWorker : ParameterizedFiber<TParameters>, new()
        where TParameters : class, new()
    {
        /// <summary>
        /// Thread local parameters.
        /// </summary>
        TParameters _parameters;

        /// <summary>
        /// Thread local parameters.
        /// </summary>
        public TParameters Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        public ParameterizedFiberArray() { }

        public ParameterizedFiberArray(int noW, TParameters parameters)
            : base(noW)
        {
            _parameters = parameters;
            for (int i = 0; i < NumberOfWorkers; i++)
                Workers.Add(new TWorker {Parameters = _parameters});
        }
    }

    /// <summary>
    /// Array of parameterless fibers.
    /// </summary>
    /// <typeparam name="TWorker">Parameterless fiber type.</typeparam>
    public class ParameterlessFiberArray<TWorker> :
        FiberArray<TWorker>
        where TWorker : ParameterlessFiber, new()
    {
        public ParameterlessFiberArray() { }

        public ParameterlessFiberArray(int noW)
            : base(noW)
        {
            for (int i = 0; i < NumberOfWorkers; i++)
                Workers.Add(new TWorker());
        }
    }

    /// <summary>
    /// Array of fibers.
    /// </summary>
    /// <typeparam name="TWorker">Fiber derived type.</typeparam>
    public class FiberArray<TWorker> :
        Synchronizer,
        ISynchronizable
        where TWorker : class, ISynchronizable
    {
        #region Fields

        /// <summary>
        /// The fibers of the array.
        /// </summary>
        protected List<TWorker> Workers;

        /// <summary>
        /// The number of workers.
        /// </summary>
        protected int NumberOfWorkers;

        #endregion // End of Fields

        #region Initialization

        public FiberArray() { }

        public FiberArray(int noW)
        {
            if (noW <= 0) return;
            NumberOfWorkers = noW;
            Workers = new List<TWorker>(NumberOfWorkers);
        }

        #endregion // End of Initialization

        #region START / STOP / CLOSE

        /// <summary>
        /// Starts the fiber array.
        /// </summary>
        /// <returns>Error codes for the fiber array: 
        /// 0 (thread started), 
        /// -1 (thread allready started), 
        /// -2 (out of memory), 
        /// -3 (null parameters). 
        /// Error codes for the i-th fiber: 
        /// error code as above * (worker number + 1)</returns>
        public int Start()
        {
            int error, i = 0;
            do
            {
                error = Workers[i].Start();
                i++;
            }
            while (i < NumberOfWorkers && error == 0);
            if (error != 0)
                error = error*(i + 2);
            return error;
        }

        /// <summary>
        /// Starts the exiting procedure of the array.
        /// </summary>
        public void Stop()
        {
            for (int i = 0; i < NumberOfWorkers; i++)
                Workers[i].Stop();
        }

        /// <summary>
        /// Wait for exit and release resources of the array.
        /// </summary>
        public void Close()
        {
            int i = 0;

            while (NumberOfWorkers != 0)
            {
                if (i >= NumberOfWorkers)
                {
                    i = 0;
                    Thread.Sleep(100);
                }
                if (Workers[i].Exited)
                {
                    Workers[i].Close();
                    Workers.RemoveAt(i);
                    NumberOfWorkers--;
                }
                else i++;
            }
        }

        #endregion // End of START / STOP / CLOSE

        #region PAUSE / UNPAUSE

        /// <summary>
        /// Returns the paused state of the fiber array.
        /// </summary>
        public bool Paused
        {
            get { lock (IsPausedLock) return Workers.All(worker => worker.Paused); }
        }

        /// <summary>
        /// Accesses the fiber array's requested paused state.
        /// </summary>
        public bool PauseRequested
        {
            get { lock (IsPauseRequestedLock) return IsPauseRequested; }
            set
            {
                lock (IsPauseRequestedLock)
                {
                    IsPauseRequested = value;
                    foreach (TWorker worker in Workers)
                        worker.PauseRequested = value;
                }
            }
        }

        #endregion // End of PAUSE / UNPAUSE

        #region EXIT

        /// <summary>
        /// Returns wheather exit has been requested.
        /// </summary>
        public bool ExitRequested
        {
            get { lock (IsExitRequestedLock) return IsExitRequested; }
            set
            {
                lock (IsExitRequestedLock)
                {
                    IsExitRequested = value;
                    foreach (TWorker worker in Workers)
                        worker.ExitRequested = value;
                }   
            }
        }

        /// <summary>
        /// Returns wheather the fiber array has exited.
        /// </summary>
        public bool Exited
        {
            get { lock (IsExitedLock) return Workers.All(worker => worker.Exited); }
        }

        #endregion // End of EXIT

        /// <summary>
        /// Wakes up the fiber array.
        /// </summary>
        public void WakeUp()
        {
            int i = 0;
            bool more = true;

            while (more && i < Workers.Count)
            {
                TWorker worker = Workers[i];
                if (worker.PauseRequested)
                {
                    worker.PauseRequested = false;
                    more = false;
                }
                else i++;
            }
        }
    }
}
