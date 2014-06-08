using System.Collections;
using System.ComponentModel;
using GeoServices;

namespace BoxHost
{
    [RunInstaller(true)]
    public partial class BoxServiceInstaller : System.Configuration.Install.Installer
    {
        public BoxServiceInstaller()
        {
            InitializeComponent();
            serviceInstaller.ServiceName = Configuration.ServiceName;
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            Calcul.ator.LogNotification("Service installing...");
            base.OnBeforeInstall(savedState);
            Calcul.ator.StartLogger();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            Hashtable args = Configuration.GetCommandLineArguments();
            if (args != null && args.ContainsKey("autostart") && (bool)args["autostart"])
            {
                stateSaver.Add("start", true);
                if (args.ContainsKey("args"))
                    stateSaver.Add("args", args["args"]);
            }
            Configuration.Clear();
            Calcul.ator.LogNotification("Service installed.");
        }

        protected override void OnBeforeRollback(IDictionary savedState)
        {
            Calcul.ator.LogNotification("Service install rolling back...");
            base.OnBeforeRollback(savedState);
        }

        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
            Calcul.ator.LogNotification("Service install rolled back.");
            Calcul.ator.StopLogger();
        }

        protected override void OnCommitting(IDictionary savedState)
        {
            Calcul.ator.LogNotification("Service committing...");
            base.OnCommitting(savedState);
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
            Calcul.ator.LogNotification("Service committed.");
            if (savedState.Contains("start") && (bool)savedState["start"])
            {
                if (savedState.Contains("args"))
                    Utilities.StartService(((string) savedState["args"]).Split('|'));
                else Utilities.StartService();
            }
            Calcul.ator.StopLogger();
        }

        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            Calcul.ator.LogNotification("Service uninstalling...");
            base.OnBeforeUninstall(savedState);
            Calcul.ator.StartLogger();
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
            Calcul.ator.LogNotification("Service uninstalled.");
            Calcul.ator.StopLogger();
        }
    }
}
