using System;
using System.Collections;
using System.Configuration.Install;
using System.Diagnostics;
using System.ServiceProcess;

namespace BoxHost
{
    static class Utilities
    {
        #region Constants

        /// <summary>
        /// The time to wait for service start.
        /// </summary>
        const double StartWaitTime = 10;

        /// <summary>
        /// The time to wait for service pause.
        /// </summary>
        const double PauseWaitTime = 10;

        /// <summary>
        /// The time to wait for service unpause.
        /// </summary>
        const double ContinueWaitTime = 10;

        /// <summary>
        /// The time to wait for service stop.
        /// </summary>
        const double StopWaitTime = 10;

        #endregion

        /// <summary>
        /// Installs the service and starts it immediatelly after if non-null arguments are passed in.
        /// </summary>
        /// <param name="args">Command line arguments for the service initialization.</param>
        /// <returns>Tool error code.</returns>
        public static int InstallService(string[] args = null)
        {
            if (IsInstalled()) return (int)ToolError.NoError;
            if (args != null)
                Configuration.SetCommandLineArguments(new Hashtable(2)
                {{"autostart", true}, {"args", string.Join("|", args)}});
            try
            {
                using (Installer installer = GetInstaller())
                {
                    IDictionary state = new Hashtable();
                    try
                    {
                        installer.Install(state);
                        installer.Commit(state);
                    }
                    catch 
                    {
                        try { installer.Rollback(state); }
                        catch { return (int)ToolError.InstallServiceNotRollBacked; }
                        return (int)ToolError.InstallService;
                    }
                }
            }
            catch { return (int)ToolError.InstallService; }

            return (int)ToolError.NoError;
        }

        /// <summary>
        /// Uninstalls the service.
        /// </summary>
        /// <returns>Tool error code.</returns>
        public static int UninstallService()
        {
            if (!IsInstalled()) return (int)ToolError.NoError;
            try
            {
                using (Installer installer = GetInstaller())
                {
                    try { installer.Uninstall(new Hashtable()); }
                    catch { return (int)ToolError.UnistallService; }
                }
            }
            catch { return (int)ToolError.UnistallService; }

            return (int)ToolError.NoError;
        }

        /// <summary>
        /// Get the assembly of the service.
        /// </summary>
        /// <returns>The installer.</returns>
        private static AssemblyInstaller GetInstaller()
        {
            var installer = new AssemblyInstaller(typeof(BoxServiceHost).Assembly, null) {UseNewContext = true};
            SetInstallers(installer);
            return installer;
        }

        /// <summary>
        /// Set the unistall action of the event log installers to remove.
        /// </summary>
        /// <param name="installer">The installer to proccess.</param>
        private static void SetInstallers(Installer installer)
        {
            InstallerCollection installers = installer.Installers;

            foreach (Installer I in installers)
            {
                var eventLogInstall = I as EventLogInstaller;
                if (eventLogInstall != null)
                {
                    eventLogInstall.UninstallAction = UninstallAction.Remove;
                    break;
                }
                SetInstallers(I);
            }
        }

        /// <summary>
        /// Starts the service.
        /// </summary>
        /// <param name="args">The command line arguments to pass to the service's start method.</param>
        /// <returns>Tool error code.</returns>
        public static int StartService(string[] args = null)
        {
            if (!IsInstalled()) return (int)ToolError.ServiceNotInstalled;
            using (var controller = new ServiceController(Configuration.ServiceName))
            {
                try
                {
                    if (controller.Status != ServiceControllerStatus.Running)
                    {
                        if (args != null) controller.Start(args);
                        else controller.Start();
                        controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(StartWaitTime));
                    }
                }
                catch (System.ServiceProcess.TimeoutException) { return (int)ToolError.StartServiceTimeOut; }
                catch (Exception) { return (int)ToolError.StartService; }
            }
            return (int)ToolError.NoError;
        }

        /// <summary>
        /// Pauses the service.
        /// </summary>
        /// <returns>Tool error code.</returns>
        public static int PauseService()
        {
            if (!IsInstalled()) return (int)ToolError.ServiceNotInstalled;
            using (var controller = new ServiceController(Configuration.ServiceName))
            {
                try
                {
                    if (controller.Status == ServiceControllerStatus.Running)
                    {
                        controller.Pause();
                        controller.WaitForStatus(ServiceControllerStatus.Paused, TimeSpan.FromSeconds(PauseWaitTime));
                    }
                }
                catch (System.ServiceProcess.TimeoutException) { return (int)ToolError.PauseServiceTimeOut; }
                catch (Exception) { return (int)ToolError.PauseService; }
            }
            return (int)ToolError.NoError;
        }

        /// <summary>
        /// Continues the service.
        /// </summary>
        /// <returns>Tool error code.</returns>
        public static int ContinueService()
        {
            if (!IsInstalled()) return (int)ToolError.ServiceNotInstalled;
            using (var controller = new ServiceController(Configuration.ServiceName))
            {
                try
                {
                    if (controller.Status == ServiceControllerStatus.Paused)
                    {
                        controller.Continue();
                        controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(ContinueWaitTime));
                    }
                }
                catch (System.ServiceProcess.TimeoutException) { return (int)ToolError.ContinueServiceTimeOut; }
                catch (Exception) { return (int)ToolError.ContinueService; }
            }
            return (int)ToolError.NoError;
        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        /// <param name="uninstall">Specifies whether to unistall the service after it has been stopped.</param>
        /// <returns>Tool error code.</returns>
        public static int StopService(bool uninstall = false)
        {
            if (!IsInstalled()) return (int)ToolError.ServiceNotInstalled;
            using (var controller = new ServiceController(Configuration.ServiceName))
            {
                try
                {
                    if (controller.Status != ServiceControllerStatus.Stopped)
                    {
                        controller.Stop();
                        controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(StopWaitTime));
                    }
                }
                catch (System.ServiceProcess.TimeoutException) { return (int)ToolError.StopServiceTimeOut; }
                catch (Exception) { return (int)ToolError.StopService; }
            }
            if (uninstall) return UninstallService();
            return (int)ToolError.NoError;
        }

        /// <summary>
        /// Checks if the service is installed.
        /// </summary>
        /// <returns>Whether the service is installed.</returns>
        public static bool IsInstalled()
        {
            if (GetServiceStatus() == null) return false;
            return true;
        }

        /// <summary>
        /// Checks if the service is running.
        /// </summary>
        /// <returns>Whether the service is running.</returns>
        public static bool IsRunning()
        {
            if (GetServiceStatus() == ServiceControllerStatus.Running) return true;
            return false;
        }

        private static ServiceControllerStatus? GetServiceStatus()
        {
            using (var controller = new ServiceController(Configuration.ServiceName))
            {
                try { return controller.Status; }
                catch { return null; }
            }
        }
    }
}
