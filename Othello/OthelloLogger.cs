using System.Diagnostics;
using System.IO;

namespace Othello
{
    /// <summary>
    /// A singleton class for logging outputs using the singleton design pattern
    /// </summary>
    public class OthelloLogger
    {
        private static OthelloLogger othellologger = new OthelloLogger();
        private readonly string logpath = "othellolog.txt";
        private static readonly string loggername = "othellologger";
        private static Stream myFile;
        private static object syncObj = new object();

        private OthelloLogger()
        {
            myFile = File.Create(logpath);
            Trace.Listeners.Add(new TextWriterTraceListener(myFile, loggername));
            Trace.AutoFlush = true;
            Trace.WriteLine("Instantiated OthelloLogger Singleton.");
        }

        /// <summary>
        /// Gets the instance to othello logger. Calling this function will instantiate OthelloLogger as Singleton.
        /// </summary>
        /// <returns></returns>
        public static OthelloLogger GetInstance()
        {
            return othellologger;
        }

        /// <summary>
        /// Disable Logging
        /// </summary>
        public static void Disable()
        {
            Trace.WriteLine("Disabled Trace Logging.");
            Trace.Close();
        }

        /// <summary>
        /// Enable Logging
        /// </summary>
        public static void Enable()
        {
            lock (syncObj)
            {
                if (Trace.Listeners.Count == 0)
                {
                    Trace.Listeners.Add(new TextWriterTraceListener(myFile, loggername));
                    Trace.WriteLine("Enabled Trace Logging.");
                }
            }
        }

    }
}
