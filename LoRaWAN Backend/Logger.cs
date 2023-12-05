using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace LoRaWAN
{
    public class Logger
    {
        private static string m_exePath = string.Empty;

        //Use for more flexibility, e.g. multible logs with multible instances
        /*
        public Logger(string logMessage)
        {
            LogWrite(logMessage);
        }
        */

        public static void LogWrite(string logMessage, string serverName)
        {
            // More flexibility, adds a log file inside each assembly location
            // m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            m_exePath = Path.Combine(Environment.CurrentDirectory, "..", @"LoRaWAN Backend\Logs");
            try
            {
                using (StreamWriter w = File.AppendText(m_exePath + "\\" + "log.txt"))
                {
                    Log(logMessage, serverName, w);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static void LogWriteSent(string logMessage, string serverName, string destination)
        {
            // More flexibility, adds a log file inside each assembly location
            // m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            m_exePath = Path.Combine(Environment.CurrentDirectory, "..", @"LoRaWAN Backend\Logs");
            try
            {
                using (StreamWriter w = File.AppendText(m_exePath + "\\" + "log.txt"))
                {
                    LogSent(logMessage, serverName, destination, w);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private static void Log(string logMessage, string serverName, TextWriter txtWriter)
        {
            try
            {
                if (IsJsonString(logMessage))
                {
                    var logData = JsonConvert.DeserializeObject<LogData>(logMessage);

                    txtWriter.Write("\r\nLog Entry : ");
                    txtWriter.WriteLine("{0:dd. MMMM yyyy, dddd} {1:HH:mm:ss.fff}",
                        DateTime.Now, DateTime.Now);
                    txtWriter.WriteLine("  >>Server: {0}", serverName);
                    txtWriter.WriteLine("  >>received Packet contains: ");
                    txtWriter.WriteLine("  >>Transaction ID: {0}", logData.TransactionID);
                    txtWriter.WriteLine("  >>{0}", logMessage);
                    txtWriter.WriteLine("--------------------------------------------------");
                }
                else if(IsHexString(logMessage))
                {
                    // Convert hex to ASCII
                    string asciiString = HexToAscii(logMessage);

                    txtWriter.Write("\r\nLog Entry : ");
                    txtWriter.WriteLine("{0:dd. MMMM yyyy, dddd} {1:HH:mm:ss.fff}",
                        DateTime.Now, DateTime.Now);
                    txtWriter.WriteLine("  >>Server: {0}", serverName);
                    txtWriter.WriteLine("  >>received Packet contains: ");
                    txtWriter.WriteLine("  >>{0}", logMessage.Replace("-", ""));
                    txtWriter.WriteLine("  >>{0}", asciiString);
                    txtWriter.WriteLine("--------------------------------------------------");

                }
                else 
                { 
                    
                }
                
            }
            catch (Exception ex)
            {
            }
        }

        public static void LogSent(string logMessage, string serverName, string destination, TextWriter txtWriter)
        {
            try
            {
                if (IsJsonString(logMessage))
                {
                    var logData = JsonConvert.DeserializeObject<LogData>(logMessage);

                    txtWriter.Write("\r\nLog Entry : ");
                    txtWriter.WriteLine("{0:dd. MMMM yyyy, dddd} {1:HH:mm:ss.fff}",
                        DateTime.Now, DateTime.Now);
                    txtWriter.WriteLine("  >>Server: {0}", serverName);
                    txtWriter.WriteLine("  >>Packet sent to {0}", destination);
                    txtWriter.WriteLine("  >>Transaction ID: {0}", logData.TransactionID);
                    txtWriter.WriteLine("  >>{0}", logMessage);
                    txtWriter.WriteLine("--------------------------------------------------");
                }
                else if (IsHexString(logMessage))
                {
                    // Convert hex to ASCII
                    string asciiString = HexToAscii(logMessage);

                    txtWriter.Write("\r\nLog Entry : ");
                    txtWriter.WriteLine("{0:dd. MMMM yyyy, dddd} {1:HH:mm:ss.fff}",
                        DateTime.Now, DateTime.Now);
                    txtWriter.WriteLine("  >>Server: {0}", serverName);
                    txtWriter.WriteLine("  >>Packet sent to {0}", destination);
                    txtWriter.WriteLine("  >>received Packet contains: ");
                    txtWriter.WriteLine("  >>{0}", logMessage.Replace("-", ""));
                    txtWriter.WriteLine("  >>{0}", asciiString);
                    txtWriter.WriteLine("--------------------------------------------------");

                }
                else
                {

                }
            }
            catch (Exception ex)
            {
            }
        }

        private class LogData
        {
            public int TransactionID { get; set; }
            // Add other properties from the logMessage JSON if needed
        }

        static bool IsHexString(string logMessage)
        {
            logMessage = logMessage.Replace("-", "");
            return Regex.IsMatch(logMessage, "^[0-9A-Fa-f]+$");
        }

        static bool IsJsonString(string logMessage)
        {
            try
            {
                JToken.Parse(logMessage);
                return true;
            }
            catch
            {
                return false;
            }
        }

        static string HexToAscii(string logMessage) 
        {
            logMessage = logMessage.Replace("-", "");
            StringBuilder ascii = new StringBuilder();

            // Finden Sie den Index des Zeichens '{' in logMessage
            int openingCurlyBraceIndex = logMessage.IndexOf("7B");
            if (openingCurlyBraceIndex != -1)
            {
                for (int i = openingCurlyBraceIndex; i < logMessage.Length; i += 2)
                {
                    string hs = logMessage.Substring(i, 2);
                    ascii.Append(Convert.ToChar(Convert.ToUInt32(hs, 16)));
                }
            }
            return ascii.ToString();

        }
    }
}