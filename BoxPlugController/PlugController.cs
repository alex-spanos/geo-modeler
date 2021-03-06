﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BoxPlugBridge;

namespace BoxPlugController
{
    public class PlugController : IDisposable
    {
		/// <summary>
		/// Fired when the plug-in assemblies are successfully loaded into the 
		/// AppDomain.
		/// </summary>
		public event EventHandler PluginsLoaded;
		/// <summary>
		/// Fired when the plug-in assemblies and their AppDomain are unloaded.
		/// </summary>
		public event EventHandler PluginsUnloaded;

		/// <summary>
		/// Gets or sets the AppDomain into which the plug-ins are loaded.
		/// </summary>
		private AppDomain Domain { get; set; }
		/// <summary>
		/// Gets or sets the PluginLoader which is responsible for 
		/// instantiating and returning objects from the AppDomain.
		/// </summary>
        private IPlugBuilder Builder { get; set; }
		/// <summary>
		/// Gets or sets the ISponsor implementation that prevents the loader 
		/// object from being disconnected.
		/// </summary>
        private Sponsor<IPlugBuilder> Sponsor { get; set; }
		/// <summary>
		/// Gets or sets the path to the plug-in assemblies.
		/// </summary>
		public string PluginPath { get; set; }
		/// <summary>
		/// Gets whether the instance has been disposed.
		/// </summary>
		public bool IsDisposed { get; private set; }

		/// <summary>
		/// Initialises a new instance of the PluginHost class.
		/// </summary>
		public PlugController() {}

		/// <summary>
		/// Initalises a new instance of the PluginHost class using the specified 
		/// path to the plugin assemblies.
		/// </summary>
		/// <param name="path"></param>
		public PlugController(string path)
        {
			PluginPath = path;
		}

		/// <summary>
		/// Finalizer.
		/// </summary>
		~PlugController()
        {
			Dispose(false);
		}

		/// <summary>
		/// Returns the type of the first class that implements the specified interface in all loaded assemblies.
		/// </summary>
		/// <typeparam name="TInterface"></typeparam>
		/// <returns></returns>
		public Sponsor<TInterface> GetImplementation<TInterface>()
            where TInterface : class
        {
			TInterface instance = Builder.GetImplementation<TInterface>();

			return instance != null ? new Sponsor<TInterface>(instance) : null;
        }

		/// <summary>
		/// Returns the types of all classes that implement the specified interface across all loaded assemblies
		/// </summary>
		/// <param name="pInterface"></param>
		/// <returns></returns>
		public IEnumerable<Sponsor<TInterface>> GetImplementations<TInterface>()
            where TInterface : class
        {
			var instances = new LinkedList<Sponsor<TInterface>>();

		    foreach (TInterface instance in Builder.GetImplementations<TInterface>())
		        instances.AddLast(new Sponsor<TInterface>(instance));

		    return instances;
		}

		/// <summary>
		/// Returns the name of the assembly that owns the specified object instance. Due to crossing 
		/// AppDomain boundaries, you cannot use the Object.GetType() method to determine the type.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public AssemblyName GetOwningAssembly(object instance)
        {
			return Builder.GetOwningAssembly(instance);
		}

		/// <summary>
		/// Returns the name of the type that was used to instantiate the specified object instance. 
		/// Due to crossing AppDomain boundaries, you cannot use the Object.GetType() method to 
		/// determine the type.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public string GetTypeName(object instance)
        {
			return Builder.GetTypeName(instance);
		}

		/// <summary>
		/// Loads (or reloads) all valid plug-in assemblies in the configured plug-in directory
		/// </summary>
		public bool LoadPlugins()
        {
			bool success = false;

			// unload any existing AppDomain and create a new one for the plugins
			UnloadDomain();
			CreateDomain();

			// only attempt to load plugins if the plugin directory exists
			if (Directory.Exists(PluginPath))
            {
				// load plugins in the other AppDomain
				Builder.Init(PluginPath);
				success = true;
			}

			// raise the PluginsLoaded event
			if (success && (PluginsLoaded != null)) PluginsLoaded(null, EventArgs.Empty);

			return success;
		}

		/// <summary>
		/// Creates the AppDomain used for loading plugins, as well as the sandbox and sponsor objects.
		/// </summary>
		private void CreateDomain()
        {
			if (Domain == null)
            {
				// create another AppDomain for loading the plug-ins
				var setup = new AppDomainSetup
				{
				    ApplicationBase = Path.GetDirectoryName(typeof (IPlugBuilder).Assembly.Location),
                    // plug-ins are isolated on the file system as well as the AppDomain
				    PrivateBinPath = PluginPath,
				    DisallowApplicationBaseProbing = false,
				    DisallowBindingRedirects = false
				};
                Domain = AppDomain.CreateDomain("Plugin AppDomain", null, setup);
            }

			// instantiate PluginLoader in the other AppDomain
		    Builder = (IPlugBuilder) Domain.CreateInstanceAndUnwrap(
		        typeof (IPlugBuilder).Assembly.FullName,
		        typeof (IPlugBuilder).FullName);

			// since Sandbox was loaded from another AppDomain, we must sponsor 
			// it for as long as we need it
            Sponsor = new Sponsor<IPlugBuilder>(Builder);
		}

		/// <summary>
		/// Unloads the AppDomain used for plugins.
		/// </summary>
		private void UnloadDomain()
        {
		    if (Domain == null) return;

		    Sponsor.Dispose();
		    Builder = null;

		    AppDomain.Unload(Domain);
		    Domain = null;

		    // raise the PluginsUnloaded event
		    if (PluginsUnloaded != null)
                PluginsUnloaded(null, EventArgs.Empty);
        }

		/// <summary>
		/// Disposes the instance.
		/// </summary>
		public void Dispose()
        {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Disposes the instance.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
            if (IsDisposed || !disposing) return;
		    UnloadDomain();
		    IsDisposed = true;
		}
    }
}
