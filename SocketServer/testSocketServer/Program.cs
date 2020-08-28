using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Config/Log.config", Watch = true)]

namespace testSocketServer
{
    class Program
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            SocketServer.EventServer appServer = new SocketServer.EventServer();

            SuperSocket.SocketBase.Config.ServerConfig serverConfig = new SuperSocket.SocketBase.Config.ServerConfig
            {
                Ip = "0.0.0.0",
                Port = 8800,
                MaxRequestLength = 2048,            //1024
                // TextEncoding = "UTF-8"
                MaxConnectionNumber = 100,
                //SendingQueueSize = 20000,         //5
                SyncSend = true,
                SendTimeOut = 2000
            };

            if (!appServer.Setup(serverConfig))
            {
                Console.WriteLine("Failed to setup");
                return;
            }
            if(!appServer.Start())
            {
                Console.WriteLine("Failed to start");
                return;
            }
            Console.WriteLine("server start successfully, press key 'q' to stop it!");
            //while(Console.ReadKey().KeyChar != 'q')
            //{
            //    Console.WriteLine();
            //    continue;
            //}
            string input = Console.ReadLine();
            while(input != "q")
            {
                switch(input)
                {
                    case "1":
                        TestSendAlotToAllClient(appServer);
                        break;
                    case "2":
                        TestSend10ToAll(appServer);
                        break;
                }
                input = Console.ReadLine();
            }

            appServer.Stop();
            Console.WriteLine("server stoped");
        }

        private static void TestSendAlotToAllClient(SocketServer.EventServer eventServer)
        {
            log4j.Info("start sending");
            eventServer.TestSendAlot();
            log4j.Info("finish");
        }

        private static void TestSend10ToAll(SocketServer.EventServer eventServer)
        {
            log4j.Info("start sending");
            eventServer.TestSend10();
            log4j.Info("finish");
        }
    }
}
