using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            WebSocket4Net.WebSocket websocket = new WebSocket4Net.WebSocket("ws://127.0.0.1:2012");

            websocket.Opened += new EventHandler((s, e) =>
            {
                log4j.Info("Opened");
            });
            websocket.Closed += new EventHandler((s, e) =>
            {
                log4j.Info("Closed");
            });
            websocket.MessageReceived += new EventHandler<WebSocket4Net.MessageReceivedEventArgs>((s, e) =>
            {
                log4j.Info("received from server: " + e.Message);
            });


            websocket.Open();
            log4j.Info(websocket.State);
            
            string cmd = "";
            while (cmd != "q")
            {

                //websocket.
                if(cmd != "")
                {
                    websocket.Send(cmd);
                }
                cmd = Console.ReadLine();
            }

            websocket.Close();
        }
    }
}
