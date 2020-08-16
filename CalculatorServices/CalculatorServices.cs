using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using WsdlClient;

namespace CalculatorServices 
{

    public class CalculatorServices
    {
        private static string sDebugFileEnv = "WEBCLIENTDEBUGLOG";

        private static Action<string> logHandler;

        public CalculatorServices()
        {
            string sDebug = "";
            try 
            {
                sDebug = Environment.GetEnvironmentVariable(sDebugFileEnv);
                if (sDebugFileEnv == "1")
                {
                    logHandler =  Utilities.StreamWriteline;
                    logHandler += Console.WriteLine;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("No Debug Env Variable set {0} {1}", sDebugFileEnv, ex.Message);
            }
        }

        public static void SetLogPath(string sFilePath, bool bCosole = true)
        {
            Utilities.LogPath = sFilePath;
            if (bCosole == false)
            {
                logHandler -= Console.WriteLine;
            }
        }
        public string GetLogPath()
        {
            return Utilities.LogPath;
        }

        public int Add(int first, int second, out int result)
        {
            CalculatorSoap client = new CalculatorSoap();
            try 
            {
                int result = client.Add(first, second);
            }
            catch(Exception ex)
            {
                Utilites.LogMessage(string.Format("The result for Add function could not be retrieved. {0}", ex.Message), logHandler);
                client.Close();
                return -1;
            }
            client.Close();
            return 0;
        }
    }
}