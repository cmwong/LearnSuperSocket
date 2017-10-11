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

            ////WebSocket4Net.JsonWebSocket websocket = new WebSocket4Net.JsonWebSocket("ws://127.0.0.1:2012");
            string serverIP = ConfigurationManager.AppSettings["ServerIP"].ToString();
            string serverPort = ConfigurationManager.AppSettings["ServerPort"].ToString();

            ////WebSocket4Net.WebSocket websocket = new WebSocket4Net.WebSocket(string.Format("ws://{0}:{1}", serverIP, serverPort));
            //WebSocket.Client.MyJsonWebSocket websocket = new WebSocket.Client.MyJsonWebSocket(string.Format("ws://{0}:{1}", serverIP, serverPort));

            //websocket.Opened += new EventHandler((s, e) =>
            //{
            //    log4j.Info("Opened");
            //});
            //websocket.Closed += new EventHandler((s, e) =>
            //{
            //    log4j.Info("Closed");
            //});
            //websocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>((s, e) =>
            //{
            //    log4j.Info("error", e.Exception);
            //});


            //websocket.Open();
            //log4j.Info(websocket.State);
            
            IClient.IMyClient websocket = new WebSocket.Client.MyWebSocketClient(serverIP, int.Parse(serverPort));
            websocket.TimeOutMilliSec = 8000;
            websocket.OnResponseEcho += new IClient.ResponseEchoHandler((responseEcho) =>
            {
                log4j.Info("Received echo: " + responseEcho.Message);
            });
            websocket.OnClosed += new EventHandler((s, e) =>
            {
                log4j.Info("Closed");
            });
            websocket.OnError += new EventHandler<System.IO.ErrorEventArgs>((s, e) =>
            {
                log4j.Info("error", e.GetException());
            });
            websocket.OnConnected += new EventHandler((s, e) =>
            {
                log4j.Info("connected");
            });

            websocket.Start();
            log4j.Info("client isconnected: " + websocket.IsConnected());

            string cmd = "";
            while (cmd != "q")
            {

                //websocket.
                if(cmd == "1")
                {
                    //websocket.Send(cmd);
                    try
                    {
                        WebSocket.Data.ResponseAdd responseAdd = websocket.RequestAdd(1, 2, 2);
                        log4j.Info("ResponseAdd: " + responseAdd.Result);
                    } catch(Exception ex)
                    {
                        log4j.Info("RequestAdd error: ", ex);
                    }
                } else if(cmd == "2")
                {
                    string msg = "Bla bla bla " + DateTime.Now;
                    log4j.Info("RequestEcho: " + msg);
                    websocket.RequestEcho(msg);
                }
                else if(cmd == "3")
                {
                    log4j.Info("isConnected: " + websocket.IsConnected());
                }
                cmd = Console.ReadLine();
            }
            //websocket.close();
            websocket.Stop();
        }
    }
}
