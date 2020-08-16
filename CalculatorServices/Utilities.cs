using System.Collections.Generic;
using System;
using System.IO;
using System.ServiceModel.Description;
using System.Configuration;

using WebClientUtils;

namespace CalculatorServices
{
    public class Utilities
    {
        private static StreamWriter _logStream;
        private static bool _bLogFileCreated;
        private static string _sLogPath;

        public static string _sLogPath
        {
            get
            {
                return _sLogPath;
            }
            set
            {
                Utilities._sLogPath = value;
            }
        }

        public static void LogMessage(string sMessage, Action<string> logHandler)
        {
            try 
            {
                if (logHandler != null)
                {
                    logHandler(sMessage);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error logging data to Streams");
            }
        }

        public static void StreamWriteline(string sMessage)
        {
            try
            {
                if (LogPath == null)
                {
                    _sLogPath = System.IO.Path.GetTempPath() + @"ClientLog.log";
                }
                if (_bLogFileCreated)
                {
                    _logStream = System.IO.File.AppendText(LogPath);
                }
                else
                {
                    _logStream = System.IO.File.CreateText(LogPath);
                }
                string logLine = System.String.Format("{0:G} : {1}", System.DateTime.Now, sMessage); 
                _logStream.WriteLine(logLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot write in File String log file...");
            }
            finally 
            {
                _logStream.Close();
            }
            _bLogFileCreated = true;
        }
    }
}