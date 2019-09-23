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
            myServer.Start();

            Console.ReadLine();
        }

        private static void MyServer_OnAccepted(object userToken)
        {
            IUserToken ut = (IUserToken)userToken;
            log4j.Info(ut.ID);
        }

        private static void MyServer_OnNewPackageReceived(object userToken, SAEASocket.Custom.Package e)
        {
            IUserToken ut = (IUserToken)userToken;
            log4j.Info(ut.ID + ", " + JsonConvert.SerializeObject(e));
        }

    }
}
