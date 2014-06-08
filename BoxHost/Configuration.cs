using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace BoxHost
{
    static class Configuration
    {
        public static string ServiceName { get; set; }

        public static string ConfigFilePath { get; set; }

        public static string LogFilePath { get; set; }

        public static readonly bool LogServiceNotifications, LogServiceErrors;

        static Configuration()
        {
            NameValueCollection appSettings;
            LogServiceNotifications = LogServiceErrors = false;

            try { appSettings = ConfigurationManager.AppSettings; }
            catch (ConfigurationErrorsException) { appSettings = null; }

            if (appSettings != null)
            {
                LogServiceNotifications = GetOnOffSetting(appSettings, "LogServiceNotifications", false);
                LogServiceErrors = GetOnOffSetting(appSettings, "LogServiceErrors", false);
                ServiceName = appSettings["ServiceName"];
                ConfigFilePath = appSettings["ConfigFilePath"];
            }
            if (String.IsNullOrWhiteSpace(ServiceName))
                ServiceName = new Guid().ToString();
            if (String.IsNullOrWhiteSpace(ConfigFilePath))
                ConfigFilePath = Path.Combine(AssemblyFullPath, "geo_config.txt");

            if (!(LogServiceNotifications || LogServiceErrors)) return;
            LogFilePath = appSettings["LogFilePath"];
            if (String.IsNullOrWhiteSpace(LogFilePath))
                LogFilePath = Path.Combine(AssemblyFullPath, "geo_log.txt");
        }

        public static Hashtable GetCommandLineArguments()
        {
            string data;
            try { data = File.ReadAllText(ConfigFilePath); }
            catch { return null; }
            var hash = new Hashtable();
            int i = 0, k = 0, l = data.Length;
            while (k < l)
            {
                if (k > i)
                {
                    bool a = k == l - 1;
                    if (data[k] == ';' || a)
                    {
                        string entry = data.Substring(i, k - i + (a ? 1 : 0));
                        string[] pair = entry.Split(new[] { '=' }, 2);
                        if (pair.Length == 2)
                        {
                            if (pair[0].Length == 0)
                                pair[0] = hash.Count.ToString();
                            if (pair[1].Length == 0)
                                hash.Add(pair[0], true);
                            else
                            {
                                bool b;
                                if (bool.TryParse(pair[1], out b))
                                    hash.Add(pair[0], b);
                                else hash.Add(pair[0], pair[1]);
                            }
                        }
                        else hash.Add(pair[0], true);
                        i = k + 1;
                    }
                }
                else if (data[k] == ';') i = k + 1;
                k++;
            }
            return hash;
        }

        public static void SetCommandLineArguments(Hashtable hash)
        {
            if (hash == null) return;
            string content = string.Empty;
            int c = hash.Count;
            ICollection keys = hash.Keys;
            IEnumerator enumerator = keys.GetEnumerator();
            for (int i = 0; i < c; i++)
            {
                enumerator.MoveNext();
                string s = (string)enumerator.Current;
                content += s + "=" + hash[s] + (i == c - 1 ? "" : ";");
            }
            try { File.WriteAllText(ConfigFilePath, content); }
            catch (Exception) { }
        }

        public static void Clear()
        {
            if (File.Exists(ConfigFilePath))
                try { File.Delete(ConfigFilePath); }
                catch (Exception) { }
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
    }
}
