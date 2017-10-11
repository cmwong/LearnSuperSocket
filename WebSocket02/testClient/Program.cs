using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Log.config", Watch = true)]
namespace testClient
{
    class Program
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            log4j.Info("Start Main");

            //WebSocket4Net.JsonWebSocket websocket = new WebSocket4Net.JsonWebSocket("ws://127.0.0.1:2012");
            string serverIP = ConfigurationManager.AppSettings["ServerIP"].ToString();
            string serverPort = ConfigurationManager.AppSettings["ServerPort"].ToString();

            //WebSocket4Net.WebSocket websocket = new WebSocket4Net.WebSocket(string.Format("ws://{0}:{1}", serverIP, serverPort));
            WebSocket.Client.MyJsonWebSocket websocket = new WebSocket.Client.MyJsonWebSocket(string.Format("ws://{0}:{1}", serverIP, serverPort));

            websocket.Opened += new EventHandler((s, e) =>
            {
                log4j.Info("Opened");
            });
            websocket.Closed += new EventHandler((s, e) =>
            {
                log4j.Info("Closed");
            });
            websocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>((s, e) =>
            {
                log4j.Info("error", e.Exception);
            });

            //websocket.On<WebSocket.Data.ResponseAdd>(WebSocket.Data.Cmd.TcsCommand.ResponseAdd.ToString(), (e) =>
            //{
            //    log4j.Info("OnResponseAdd: " + e.Result);
            //});
            //websocket.On<WebSocket.Data.ResponseAdd>(WebSocket.Data.Cmd.TcsCommand.ResponseAdd.ToString(), (s, e) =>
            //{
            //    log4j.Info("OnResponseAdd: " + e.Result);
            //});

            websocket.Open();
            log4j.Info(websocket.State);
            
            string cmd = "";
            while (cmd != "q")
            {

                //websocket.
                if(cmd == "1")
                {
                    //websocket.Send(cmd);
                    websocket.RequestAdd(0, 2, 2);
                }
                cmd = Console.ReadLine();
            }

            websocket.Close();
        }
    }
}
