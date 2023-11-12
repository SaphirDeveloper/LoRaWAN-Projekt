using System.IO;
using System.Reflection;

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

        public static void LogWrite(string logMessage)
        {
            // More flexibility, adds a log file inside each assembly location
            // m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            m_exePath = Path.Combine(Environment.CurrentDirectory, "..", @"LoRaWAN Backend\Logs");
            try
            {
                using (StreamWriter w = File.AppendText(m_exePath + "\\" + "log.txt"))
                {
                    Log(logMessage, w);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write("\r\nLog Entry : ");
                txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                txtWriter.WriteLine("  >>Packet contains:");
                txtWriter.WriteLine("  >>{0}", logMessage);
                txtWriter.WriteLine("--------------------------------------------------");
            }
            catch (Exception ex)
            {
            }
        }
    }
}