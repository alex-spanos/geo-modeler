using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace GeoPlugIn
{
    class GeoServiceController
    {
        private readonly string _geoToolPath;
        public bool ServiceRunning { get; private set; }

        public GeoServiceController()
        {
            ServiceRunning = false;
            _geoToolPath = ConfigurationManager.AppSettings["GeoToolPath"];
            if (String.IsNullOrWhiteSpace(_geoToolPath))
                _geoToolPath = Path.Combine(AssemblyFullPath, "GeoTool.exe");
        }

        public void LaunchService()
        {
            string consoleOutput;
            ServiceRunning = RunProcess(_geoToolPath, "/is 1 1 1", out consoleOutput) == 0;
        }

        public void ShutdownService()
        {
            string consoleOutput;
            ServiceRunning = RunProcess(_geoToolPath, "/su", out consoleOutput) != 0;
        }

        private static int RunProcess(string exePath, string args, out string consoleOut)
        {
            var exeProcess = new Process();
            var outputBuilder = new StringBuilder();
            int exitCode = -1;
            string consoleOutput = "";
            bool procExited = false;

            exeProcess.StartInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = exePath,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = args
            };
            exeProcess.EnableRaisingEvents = true;
            exeProcess.OutputDataReceived += (sender, e) => outputBuilder.Append(e.Data);
            exeProcess.Exited += (sender, eventArgs) =>
            {
                try
                {
                    exeProcess.CancelOutputRead();
                    exitCode = exeProcess.ExitCode;
                }
                finally
                {
                    exeProcess.Close();
                }
                consoleOutput = outputBuilder.ToString();
                procExited = true;
            };
            consoleOut = "";
            try
            {
                exeProcess.Start();
                exeProcess.BeginOutputReadLine();
            }
            catch
            {
                return exitCode;
            }
            while (!procExited) { Thread.Sleep(100); }

            consoleOut = consoleOutput;
            return exitCode;
        }

        private static string AssemblyFullPath
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetFullPath(path);
            }
        }
    }
}
