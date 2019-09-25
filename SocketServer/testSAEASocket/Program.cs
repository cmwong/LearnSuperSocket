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

        static void Main(string[] args)
        {
            log4j.Info("start main");

            SAEASocket.EventSocketServer myServer = new SAEASocket.EventSocketServer("127.0.0.1", 8800);
            myServer.OnNewPackageReceived += MyServer_OnNewPackageReceived;
            myServer.OnAccepted += MyServer_OnAccepted;
            myServer.OnDisconnected += MyServer_OnDisconnected;
            myServer.OnError += MyServer_OnError;
            myServer.Start();

            Console.ReadLine();
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

        private static void MyServer_OnNewPackageReceived(object userToken, SAEASocket.Custom.Package e)
        {
            SAEASocket.Custom.UserToken ut = (SAEASocket.Custom.UserToken)userToken;
            log4j.Info("sID: " + ut.ID + ", index: " + ut.Index + ", " + JsonConvert.SerializeObject(e));
        }

    }
}
