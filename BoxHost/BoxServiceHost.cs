using System;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceProcess;
using GeoServices;

namespace BoxHost
{
    public partial class BoxServiceHost : ServiceBase
    {
        string[] _args;
        ServiceHost _serviceHost;

        public BoxServiceHost()
        {
            InitializeComponent();
        }

        public BoxServiceHost(string[] args) : this()
        {
            _args = args;
        }

        public void ConsoleRun(string[] args)
        {
            string fullName = GetType().FullName;

            Console.WriteLine(fullName + "::starting...");

            OnStart(args);

            Console.WriteLine(fullName + "::ready (ENTER to exit)");
            Console.ReadLine();

            OnStop();

            Console.WriteLine(fullName + "::stopped");
        }

        protected override void OnStart(string[] args)
        {
            if (args != null && args.Length > 0) _args = args;
            Calcul.ator.LogNotification("Service starting (box initialization arguments: " + String.Join<string>(" ", _args) + ").");
            DispatchWorker(DoPreWork, PreWorkCompleted, _args);
            base.OnStart(_args);
        }

        private void DoPreWork(object sender, DoWorkEventArgs e)
        {
            int error, startingStage, endingStage;
            int[] numbersOfWorkers;

            Calcul.ator.StartLogger();
            if (_serviceHost != null) _serviceHost.Close();

            if ((error = Program.ParseBlockArgs((string[])e.Argument, out startingStage, out endingStage, out numbersOfWorkers)) == (int)ToolError.NoError &&
                (error = Calcul.ator.Setup(startingStage, endingStage, numbersOfWorkers)) == 0)
            {
                _serviceHost = new ServiceHost(typeof(GeoService));
                _serviceHost.Open();
            }
            e.Result = error;
        }

        private void PreWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var error = (int)e.Result;

            if (error == 0) Calcul.ator.LogNotification("Service started.");
            else
            {
                Calcul.ator.LogError("Service could not be started. Error code: " + error);
                Stop();
            }
        }

        protected override void OnPause()
        {
            Calcul.ator.LogNotification("Service pausing...");
            DispatchWorker(DoPauseWork, PauseWorkCompleted);
            base.OnPause();
        }

        private static void DoPauseWork(object sender, DoWorkEventArgs e)
        {
            Calcul.ator.Pause();
        }

        private static void PauseWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Calcul.ator.LogNotification("Service paused.");
        }

        protected override void OnContinue()
        {
            Calcul.ator.LogNotification("Service continues...");
            DispatchWorker(DoContinueWork, ContinueWorkCompleted);
            base.OnContinue();
        }

        private static void DoContinueWork(object sender, DoWorkEventArgs e)
        {
            Calcul.ator.Resume();
        }

        private static void ContinueWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Calcul.ator.LogNotification("Service continued.");
        }

        protected override void OnStop()
        {
            Calcul.ator.LogNotification("Service stopping...");
            DispatchWorker(DoPostWork, PostWorkCompleted);
            base.OnStop();
        }

        protected override void OnShutdown()
        {
            OnStop();
        }

        private void DoPostWork(object sender, DoWorkEventArgs e)
        {
            if (_serviceHost != null)
            {
                _serviceHost.Close();
                _serviceHost = null;
            }
            Calcul.ator.ShutDown();
        }

        private static void PostWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Calcul.ator.LogNotification("Service stopped.");
            Calcul.ator.StopLogger();
        }

        private static void DispatchWorker(DoWorkEventHandler doWork, RunWorkerCompletedEventHandler workComplete, object args = null)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += doWork;
            worker.RunWorkerCompleted += workComplete;
            if (args == null) worker.RunWorkerAsync();
            else worker.RunWorkerAsync(args);
        }
    }
}
