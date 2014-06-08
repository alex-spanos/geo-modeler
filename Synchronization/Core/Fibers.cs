using System;
using System.Threading;

namespace Synchronization.Core
{
    /// <summary>
    /// Fiber with thread-local parameters.
    /// </summary>
    /// <typeparam name="TParameters">Thread local parameters.</typeparam>
    public abstract class ParameterizedFiber<TParameters> :
        Fiber
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

        #region Initialization

        protected ParameterizedFiber() {}

        protected ParameterizedFiber(bool pauseRequestState)
            : base(pauseRequestState) {}

        protected ParameterizedFiber(TParameters parameters)
        {
            _parameters = parameters;
        }

        protected ParameterizedFiber(TParameters parameters, bool pauseRequestState)
            : base(pauseRequestState)
        {
            _parameters = parameters;
        }

        /// <summary>
        /// Starts the worker process with parameters.
        /// </summary>
        /// <returns>The error code.</returns>
        protected sealed override int LocalStart()
        {
            if (_parameters != null)
            {
                Loop = new Thread(Worker);
                Loop.Start(_parameters);
                return 0;
            }
            return -3;
        }

        #endregion // End of Initialization

        /// <summary>
        /// The method that works in a loop inside the fiber
        /// and references the thread local parameters.
        /// </summary>
        /// <param name="parameters">Thread local parameters.</param>
        protected abstract void Worker(object parameters);
    }

    /// <summary>
    /// Fiber without thread-local parameters.
    /// </summary>
    public abstract class ParameterlessFiber :
        Fiber
    {
        #region Initialization

        /// <summary>
        /// Initialize with loop not paused.
        /// </summary>
        protected ParameterlessFiber() {}

        /// <summary>
        /// Initialize with optional pause request state.
        /// </summary>
        /// <param name="pauseRequestState">Pause request state.</param>
        protected ParameterlessFiber(bool pauseRequestState)
            : base(pauseRequestState) {}

        /// <summary>
        /// Starts the worker process without parameters.
        /// </summary>
        /// <returns>The error code.</returns>
        protected sealed override int LocalStart()
        {
            Loop = new Thread(Worker);
            Loop.Start();
            return 0;
        }

        #endregion // End of Initialization

        /// <summary>
        /// The method that works in a loop inside the fiber.
        /// </summary>
        protected abstract void Worker();
    }

    /// <summary>
    /// Pausable fiber base.
    /// </summary>
    public abstract class Fiber :
        Synchronizer,
        ISynchronizable,
        IPauseable
    {
        #region Fields

        /// <summary>
        /// The working thread.
        /// </summary>
        protected Thread Loop { get; set; }

        private bool _isPaused, _isPausing,
                     _isExited;

        #endregion // End of Fields

        #region Initialization

        /// <summary>
        /// Initialize with loop not paused.
        /// </summary>
        protected Fiber()
        {
            Init();
        }

        /// <summary>
        /// Initialize with optional pause request state.
        /// </summary>
        /// <param name="initPauseRequestedState">Initial pause request state.</param>
        protected Fiber(bool initPauseRequestedState)
            : base(initPauseRequestedState)
        {
            Init();
        }

        private void Init()
        {
            _isPausing = _isPaused = _isExited = false;
        }

        #endregion // End of Initialization

        #region START /  STOP / CLOSE

        /// <summary>
        /// Creates a new thread that hosts the loop process and starts it.
        /// </summary>
        /// <returns>error code: 
        /// 0 (thread started), 
        /// -1 (thread allready started), 
        /// -2 (out of memory), 
        /// -3 (null parameters)</returns>
        public virtual int Start()
        {
            if (Loop == null ||
                Loop.ThreadState == ThreadState.Unstarted ||
                Loop.ThreadState == ThreadState.Stopped)
                try
                {
                    return LocalStart();
                }
                catch (ThreadStateException)
                {
                    return -1;
                }
                catch (OutOfMemoryException)
                {
                    return -2;
                }
            return -1;
        }

        /// <summary>
        /// Starts the worker process with or without parameters.
        /// </summary>
        /// <returns>The error code.</returns>
        protected abstract int LocalStart();

        /// <summary>
        /// Starts the exiting procedure of the fiber.
        /// </summary>
        public virtual void Stop()
        {
            ExitRequested = true;
            PauseRequested = false;
        }

        /// <summary>
        /// Wait for exit and release resources of the fiber.
        /// </summary>
        public virtual void Close()
        {
            Loop.Join();
            Loop = null;
        }

        #endregion // End of START / STOP / CLOSE

        #region PAUSE / UNPAUSE

        /// <summary>
        /// Returns the paused state of the fiber.
        /// </summary>
        public bool Paused
        {
            get { lock (IsPausedLock) return _isPaused; }
        }

        /// <summary>
        /// Pauses the fiber if it is requested.
        /// </summary>
        /// <returns>wheather the fiber has paused</returns>
        public bool PauseIfRequested()
        {
            bool hasPaused = false;

            lock (IsPauseRequestedLock) if (IsPauseRequested) _isPausing = true;
            if (_isPausing)
            {
                Pause();
                hasPaused = true;
            }
            _isPausing = false;

            return hasPaused;
        }

        /// <summary>
        /// Accesses the fiber's requested paused state.
        /// </summary>
        public bool PauseRequested
        {
            get { lock (IsPauseRequestedLock) return IsPauseRequested; }
            set
            {
                lock (IsPauseRequestedLock)
                    if (value) IsPauseRequested = true;
                    else if (IsPauseRequested)
                    {
                        IsPauseRequested = false;
                        if (_isPausing)
                        {
                            while (!Paused) Thread.Sleep(100);
                            UnPause();
                        }
                    }
            }
        }

        /// <summary>
        /// Pauses the fiber.
        /// </summary>
        protected void Pause()
        {
            lock (IsPausedLock)
            {
                _isPaused = true;
                Monitor.Wait(IsPausedLock);
            }
        }

        /// <summary>
        /// Unpauses the fiber.
        /// </summary>
        protected void UnPause()
        {
            lock (IsPausedLock) if (_isPaused)
            {
                _isPaused = false;
                Monitor.Pulse(IsPausedLock);
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
            set { lock (IsExitRequestedLock) IsExitRequested = value; }
        }

        /// <summary>
        /// Returns wheather the fiber has exited.
        /// </summary>
        public bool Exited
        {
            get { lock (IsExitedLock) return _isExited; }
            protected set { lock (IsExitedLock) _isExited = value; }
        }

        #endregion // End of EXIT

        /// <summary>
        /// Wakes up the fiber.
        /// </summary>
        public void WakeUp()
        {
            PauseRequested = false;
        }
    }
}
