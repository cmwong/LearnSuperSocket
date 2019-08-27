using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Log.config", Watch = true)]
namespace testSocketClient
{
    class Program
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // private static SocketClient.EventSocket eventSocket;
        static void Main(string[] args)
        {
            log4j.Info("start main");

            SocketClient.EventSocket eventSocket = new SocketClient.EventSocket("127.0.0.1", 8800);
            eventSocket.NewPackageReceived += EventSocket_NewPackageReceived;
            eventSocket.Connected += EventSocket_Connected;
            eventSocket.Error += EventSocket_Error;
            eventSocket.Closed += EventSocket_Closed;

            Task<bool> ts = eventSocket.Connect();
            ts.Wait();
            log4j.Info("after ts.Wait() - " + ts.Result);
            if (!ts.Result)
            {
                return;
            }
            //ushort max = ushort.MaxValue;
            //string body = "0";
            //for (ushort i = 0; i < max; i++)
            //{
            //    body = body + i.ToString();
            //    eventSocket.Send(1, i, body);
            //}
            //Console.WriteLine("finish for loop");
            string input = Console.ReadLine();
            while(input != "q")
            {
                switch(input)
                {
                    case "1":
                        TestSendAlot(eventSocket);
                        break;
                    case "2":
                        Send_1_2(eventSocket);
                        break;
                }

                input = Console.ReadLine();
            }
        }

        private static void TestSendAlot(SocketClient.EventSocket eventSocket)
        {
            ushort key = 1;
            ushort subKey = 255;
            string data = "this is some text 爸爸 ";
            int count = 10000;

            log4j.Info("TestSendAlot " + count);
            for(int i = 0; i < count; i++)
            {
                eventSocket.Send(key, subKey, data + ": " + i);
            }
            log4j.Info("finish");
        }
        private static void Send_1_2(SocketClient.EventSocket eventSocket)
        {
            log4j.Info("sending 1_2");
            eventSocket.Send(1, 2, "rubbish");
            log4j.Info("finish sending 1_2");
        }

        private static void EventSocket_Closed(object sender, EventArgs e)
        {
            log4j.Info("socket closed");
        }

        private static void EventSocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            log4j.Info("", e.Exception);
        }

        private static void EventSocket_Connected(object sender, EventArgs e)
        {
            SocketClient.EventSocket eventSocket = (SocketClient.EventSocket)sender;
            log4j.Info("connected.");
            // eventSocket.Send(1, 1, "1_1");
        }

        private static void EventSocket_NewPackageReceived(object sender, SuperSocket.ClientEngine.PackageEventArgs<SocketClient.PackageInfo.EventPackageInfo> e)
        {
            SocketClient.PackageInfo.EventPackageInfo pkg = e.Package;

            log4j.Info($"k: {pkg.MainKey}, sk: {pkg.SubKey}, b: {pkg.Body}");
        }
    }
}
