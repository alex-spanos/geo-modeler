using System;
using System.Collections.Generic;
using System.IO;
using Synchronization.Core;
using Synchronization.Models;

namespace BoxBuilder
{
    public class Logger :
        ParameterizedFiber<LoggerParameters>
    {
        public Logger(string logFilePath)
        {
            Parameters = new LoggerParameters(new TsQueue<Tuple<DateTime, string>>(this), logFilePath);
        }

        public void Put(string logEntry)
        {
            Parameters.Queue.Enqueue(new Tuple<DateTime, string>(DateTime.Now, logEntry));
        }

        protected override void Worker(object parameters)
        {
            var logList = new List<Tuple<DateTime,string>>();

            while (!ExitRequested || Parameters.Queue.Count > 0)
            {
                bool hasPaused = PauseIfRequested();
                if (hasPaused && ExitRequested) break;

                Tuple<DateTime, string> logEntry;
                while (Parameters.Queue.DequeueIfAny(out logEntry))
                    logList.Add(logEntry);

                if (logList.Count > 0)
                {
                    try
                    {
                        File.AppendAllLines(Parameters.LogFilePath, logList.ConvertAll
                            (e => e.Item1.ToString("yyyy/MM/dd|HH:mm:ss:fff") + " > " + e.Item2));
                    }
                    catch (Exception) { }
                    logList.Clear();
                }
                else PauseRequested = true;
            }
            Exited = true;
        }
    }

    public class LoggerParameters
    {
        public TsQueue<Tuple<DateTime, string>> Queue;
        public string LogFilePath;

        public LoggerParameters() { }

        public LoggerParameters(TsQueue<Tuple<DateTime, string>> queue)
        {
            Queue = queue;
        }

        public LoggerParameters(TsQueue<Tuple<DateTime, string>> queue, string logFilePath)
            : this(queue)
        {
            LogFilePath = logFilePath;
        }
    }
}
