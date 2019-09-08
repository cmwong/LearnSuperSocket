using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Log.config", Watch = true)]

namespace testSAEASocket1
{
    class Program
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            log4j.Info("start main");

            SAEASocket.EventSocketClient eventSocketClient = new SAEASocket.EventSocketClient("127.0.0.1", 8800);
            eventSocketClient.OnError += EventSocketClient_OnError;

            eventSocketClient.ConnectAsync();

            Console.WriteLine("console waiting input");
            string input = Console.ReadLine();
        }

        private static void EventSocketClient_OnError(string ID, Exception ex)
        {
            log4j.Info(ID, ex);
        }
    }
}
