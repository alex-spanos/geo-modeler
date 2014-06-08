using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BoxBridge;

namespace BoxController
{
    /// <summary>
    /// Singleton handling the calculation box.
    /// </summary>
    public sealed class Controller :
        IBoxHandler
    {
        public string LogFilePath { get; set; }

        public bool LogServiceNotifications { get; set; }
        public bool LogServiceErrors { get; set; }
        public bool LogJobs { get; set; }

        private Box _box;

        private readonly Logger _log;

        private ConcurrentDictionary<int, Tuple<int, GeoService>> _jobList;

        private int _latestJobId;

        public static Controller I
        {
            get { return Nested.Instance; }
        }

        private Controller()
        {
            NameValueCollection appSettings;
            LogServiceNotifications = LogServiceErrors = LogJobs = false;

            try { appSettings = ConfigurationManager.AppSettings; }
            catch (ConfigurationErrorsException) { appSettings = null; }

            if (appSettings != null)
            {
                LogServiceNotifications = GetOnOffSetting(appSettings, "LogServiceNotifications", false);
                LogServiceErrors = GetOnOffSetting(appSettings, "LogServiceErrors", false);
                LogJobs = GetOnOffSetting(appSettings, "LogJobs", false);
            }
            if (!(LogServiceNotifications || LogServiceErrors || LogJobs)) return;
            LogFilePath = appSettings["LogFilePath"];
            if (String.IsNullOrWhiteSpace(LogFilePath))
                LogFilePath = Path.Combine(AssemblyFullPath, "geo_log.txt");
            _log = new Logger(LogFilePath);
        }

        public int PerformSingleCalculation(int startingStage, int endingStage, int[] numbersOfWorkers, string inputFileName, string outputFileName)
        {
            StartLogger();
            if (_box != null) ShutDown();
            LogNotification("Box starts performing single calculation.");
            _box = new Box();
            int err = _box.SingleCalculation(startingStage, endingStage, numbersOfWorkers, inputFileName, outputFileName);
            if (err == (int)BoxError.NoError) LogNotification("Box finished performing single calculation.");
            else LogError("Box performing single calculation failed with exit code: " + err);
            StopLogger();
            return err;
        }

        public int Setup(int startingStage, int endingStage, int[] numbersOfWorkers)
        {
            LogNotification("Box starts setting up for service.");
            _jobList = new ConcurrentDictionary<int, Tuple<int, GeoService>>();
            _box = new Box();
            int err = _box.SetupForService(startingStage, endingStage, numbersOfWorkers);
            if (err == (int)BoxError.NoError) LogNotification("Box setted up for service.");
            else LogError("Box setup for service failed with exit code: " + err);
            return err;
        }

        public void EnQueueJob(GeoService service, GeoTicketIn ticketIn)
        {
            _latestJobId++;
            if (_jobList.TryAdd(_latestJobId, new Tuple<int, GeoService>(ticketIn.TicketId, service)))
            {
                LogJob("IN: (jobID: " + _latestJobId + ", ticketID: " + ticketIn.TicketId + ", flowID: " + ticketIn.FlowId + ")");
                _box.EnQueueTicket(new BoxTicketIn
                {
                    JobId = _latestJobId,
                    FlowId = ticketIn.FlowId,
                    MemFileName = ticketIn.MemFileName
                });
            }
        }

        public void CalculationComplete(BoxTicketOut ticketOut)
        {
            Tuple<int, GeoService> jobItem;
            if (_jobList.TryRemove(ticketOut.JobId, out jobItem))
            {
                LogJob("OUT: (jobID: " + ticketOut.JobId + ", ticketID: " + jobItem.Item1 + ")");
                jobItem.Item2.Callback.ConsumeResult(new GeoTicketOut
                {
                    TicketId = jobItem.Item1,
                    MemFileName = ticketOut.MemFileName
                });
            }
        }

        public void Pause()
        {
            LogNotification("Box starting to pause.");
            if (_box != null) _box.PauseProcessing();
            LogNotification("Box paused.");
        }

        public void Resume()
        {
            LogNotification("Box starting to resume.");
            if (_box != null) _box.ResumeProcessing();
            LogNotification("Box resumed.");
        }

        public void ShutDown()
        {
            LogNotification("Box starting to close.");
            if (_box != null) _box.WaitForClose();
            LogNotification("Box closed.");
        }

        public void StartLogger()
        {
            if (_log != null) _log.Start();
        }

        public void StopLogger()
        {
            if (_log == null) return;
            _log.Stop();
            _log.Close();
        }

        public void LogNotification(string notification)
        {
            if (LogServiceNotifications) _log.Put("NOTIFICATION: " + notification);
        }

        public void LogError(string error)
        {
            if (LogServiceErrors) _log.Put("ERROR: " + error);
        }

        private void LogJob(string job)
        {
            if (LogJobs) _log.Put("JOB: " + job);
        }

        /// <summary>
        /// Reads a On/Off type configuration setting.
        /// </summary>
        /// <param name="appSettings">Application configuration settings.</param>
        /// <param name="settingName">Setting's name.</param>
        /// <param name="defaultValue">Default value in case the setting is found.</param>
        /// <returns>The configuration setting or the default value.</returns>
        private static bool GetOnOffSetting(NameValueCollection appSettings, string settingName, bool defaultValue)
        {
            string settingValue = appSettings[settingName];
            if (!String.IsNullOrWhiteSpace(settingValue))
                return settingValue.ToLower(new CultureInfo("en-US")) == "on";
            return defaultValue;
        }

        /// <summary>
        /// The directory where calculator assembly resides.
        /// </summary>
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

        private class Nested
        {
            static Nested() { }

            internal static readonly Controller Instance = new Controller();
        }
    }
}
