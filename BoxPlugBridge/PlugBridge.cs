using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Lifetime;
using System.Security.Permissions;

namespace BoxPlugBridge
{
    public interface IBoxPlugIn
    {

    }

    public interface IPlugBuilder
    {
        T GetImplementation<T>();
        IEnumerable<T> GetImplementations<T>();
        AssemblyName GetOwningAssembly(object instance);
        string GetTypeName(object instance);
        void Init(string pluginPath);
    }

    /// <summary>
    /// Wraps an instance of TInterface. If the instance is a 
    /// MarshalByRefObject, this class acts as a sponsor for its lifetime 
    /// service (until disposed/finalized). Disposing the sponsor implicitly 
    /// disposes the instance.
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    [Serializable]
    [SecurityPermission(SecurityAction.Demand, Infrastructure = true)]
    public sealed class Sponsor<TInterface> :
        ISponsor,
        IDisposable
        where TInterface : class
    {
        private TInterface _instance;

        /// <summary>
        /// Gets the wrapped instance of TInterface.
        /// </summary>
        public TInterface Instance
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException("Instance");
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        /// <summary>
        /// Gets whether the sponsor has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Initialises a new instance of the Sponsor&lt;TInterface&gt; class, 
        /// wrapping the specified object instance.
        /// </summary>
        /// <param name="instance"></param>
        public Sponsor(TInterface instance)
        {
            Instance = instance;

            if (!(Instance is MarshalByRefObject)) return;
            object lifetimeService = RemotingServices.GetLifetimeService((MarshalByRefObject) (object) Instance);
            var lease = lifetimeService as ILease;
            if (lease != null) lease.Register(this);
        }

        /// <summary>
        /// Finaliser.
        /// </summary>
        ~Sponsor()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes the sponsor and the instance it wraps.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the sponsor and the instance it wraps.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (IsDisposed) return;
            if (disposing)
            {
                var instance = Instance as IDisposable;
                if (instance != null)
                    instance.Dispose();

                if (Instance is MarshalByRefObject)
                {
                    object lifetimeService = RemotingServices.GetLifetimeService((MarshalByRefObject)(object)Instance);
                    var lease = lifetimeService as ILease;
                    if (lease != null)
                        lease.Unregister(this);
                }
            }
            Instance = null;
            IsDisposed = true;
        }

        /// <summary>
        /// Renews the lease on the instance as though it has been called normally.
        /// </summary>
        /// <param name="lease"></param>
        /// <returns></returns>
        public TimeSpan Renewal(ILease lease)
        {
            return IsDisposed ? TimeSpan.Zero : LifetimeServices.RenewOnCallTime;
        }
    }
}
