namespace Synchronization.Core
{
    public interface IPauseable
    {
        bool PauseIfRequested();
    }

    public interface ISynchronizable :
        IWakeable
    {
        int Start();
        bool Paused { get; }
        bool PauseRequested { get; set; }
        void Stop();
        bool Exited { get; }
        bool ExitRequested { get; set; }
        void Close();
    }

    public interface IWakeable
    {
        void WakeUp();
    }

    /// <summary>
    /// Provides the basic locking mechanism for exitable and pauseable components.
    /// </summary>
    public abstract class Synchronizer
    {
        #region Fields

        protected readonly object IsPausedLock = new object(),
                                  IsPauseRequestedLock = new object(),
                                  IsExitedLock = new object(),
                                  IsExitRequestedLock = new object();

        protected bool IsPauseRequested,
                       IsExitRequested;

        #endregion // End of Fields

        #region Initialization

        protected Synchronizer()
        {
            Init(false);
        }

        protected Synchronizer(bool initPauseRequestedState)
        {
            Init(initPauseRequestedState);
        }

        void Init(bool initPauseRequestedState)
        {
            IsExitRequested = false;
            IsPauseRequested = initPauseRequestedState;
        }

        #endregion // End of Initialization
    }
}
