using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

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
            eventSocketClient.OnDisconnected += EventSocketClient_OnDisconnected;
            eventSocketClient.OnNewPackageReceived += EventSocketClient_OnNewPackageReceived;
            try
            {
                eventSocketClient.ConnectAsync();
                //eventSocketClient.Connect();
            }
            catch (Exception ex)
            {
                log4j.Info("ConnectAsync", ex);
            }
            Console.WriteLine("console waiting input");
            string input = Console.ReadLine();
            while (input != "q")
            {
                switch (input)
                {
                    case "1":
                        TestSendAlot(eventSocketClient);
                        break;
                    case "2":
                        MakeAlotOfClient();
                        break;
                }

                input = Console.ReadLine();
            }

        }

        private static void EventSocketClient_OnNewPackageReceived(object sender, SAEASocket.Custom.Package package)
        {
            log4j.Info(package.Body);
        }

        private static void EventSocketClient_OnDisconnected(string ID, Exception ex)
        {
            log4j.Info(ID, ex);
        }

        private static void EventSocketClient_OnError(string ID, Exception ex)
        {
            log4j.Info(ID, ex);
        }

        private static void TestSendAlot(SAEASocket.EventSocketClient eventSocket)
        {
            //ushort key = 1;
            //ushort subKey = 255;
            string data = "this is some text 爸爸 ";
            int count = 10000;

            SAEASocket.Custom.Package package = new SAEASocket.Custom.Package
            {
                MainKey = 1,
                SubKey = 255
            };

            log4j.Info("TestSendAlot " + count);
            for (int i = 0; i < count; i++)
            {
                package.Body = data + " " + i;
                eventSocket.SendAsync(package);
            }
            log4j.Info("finish");
        }

        private static void MakeAlotOfClient()
        {
            for (int i = 0; i < 100; i++)
            {
                Task.Run(() =>
                {
                    SAEASocket.EventSocketClient eventSocketClient = new SAEASocket.EventSocketClient("127.0.0.1", 8800);
                    eventSocketClient.OnError += EventSocketClient_OnError;
                    eventSocketClient.OnDisconnected += EventSocketClient_OnDisconnected;
                    eventSocketClient.OnNewPackageReceived += EventSocketClient_OnNewPackageReceived;
                    eventSocketClient.ConnectAsync();
                });
                Thread.Sleep(10);
            }
        }
    }
}
