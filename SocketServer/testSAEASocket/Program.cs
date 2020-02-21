using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SAEA.Sockets.Interface;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Log.config", Watch = true)]

namespace testSAEASocket
{
    class Program
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static SAEASocket.EventSocketServer myServer;

        static void Main(string[] args)
        {
            log4j.Info("start main");

            myServer = new SAEASocket.EventSocketServer("127.0.0.1", 8800);
            myServer.OnNewPackageReceived += MyServer_OnNewPackageReceived;
            myServer.OnAccepted += MyServer_OnAccepted;
            myServer.OnDisconnected += MyServer_OnDisconnected;
            myServer.OnError += MyServer_OnError;
            myServer.Start();

            string input = Console.ReadLine();
            while (input != "q")
            {
                switch (input)
                {
                    case "1":
                        DisconnectClient();
                        break;
                }

                input = Console.ReadLine();
            }


        }

        private static void MyServer_OnError(string sessionID, ushort index, Exception ex)
        {
            log4j.Info("sID: " + sessionID + ", index: " + index);
        }

        private static void MyServer_OnDisconnected(string sessionID, ushort index, Exception ex)
        {
            log4j.Info("sID: " + sessionID + ", index: " + index);
        }

        private static void MyServer_OnAccepted(object userToken)
        {
            SAEASocket.Custom.UserToken ut = (SAEASocket.Custom.UserToken)userToken;
            log4j.Info("sID: " + ut.ID + ", index: " + ut.Index);
        }

        private static void MyServer_OnNewPackageReceived(object userToken, SAEASocket.Custom.Package package)
        {
            SAEASocket.Custom.UserToken ut = (SAEASocket.Custom.UserToken)userToken;
            log4j.Info("sID: " + ut.ID + ", index: " + ut.Index + ", " + JsonConvert.SerializeObject(package));

            // myServer.Send(ut.Index, e.CMD1, e.MainKey, e.SubKey, Encoding.UTF8.GetBytes(e.Body));
            myServer.SendAsync(ut.Index, package);
        }

        private static void DisconnectClient()
        {
            Console.Write("sessionID: ");
            string sessionID = Console.ReadLine();
            myServer.Disconnect(sessionID);
        }
    }
}
