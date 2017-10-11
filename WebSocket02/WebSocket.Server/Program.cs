using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SuperSocket.SocketBase;
using SuperSocket.WebSocket;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketEngine;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Config/log4net.config", Watch = true)]
namespace WebSocket.Server
{
    class Program
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static void Main(string[] args)
        {
            log4j.Info("Start Main");

            var appServer = new MyAppServer();
            //var appServer = new WebSocketServer();

            var serverConfig = new ServerConfig
            {
                Port = 2012,
                LogBasicSessionActivity = false,
                LogCommand = false
            };


            //Setup the appServer
            if (!appServer.Setup(serverConfig)) //Setup with listening port
            //if(!appServer.Setup(serverConfig))
            {
                Console.WriteLine("Failed to setup!");
                Console.ReadKey();
                return;
            }


            // WebSocketServer.cs line 531
            // if we hook to NewMessageReceived, server won't ExecuteMessage (won't execute our command)
            //appServer.NewMessageReceived += new SessionHandler<WebSocketSession, string>(AppServer_NewMessageReceived);
            //appServer.NewSessionConnected += new SessionHandler<WebSocketSession>((target) =>
            //{
            //    log4j.Info("client connected: " + target.SessionID);
            //});
            appServer.NewSessionConnected += new SessionHandler<JsonWebSocketSession>((s) =>
            {
                log4j.Info("client connected: " + s.SessionID);
            });
            appServer.SessionClosed += new SessionHandler<JsonWebSocketSession, CloseReason>((session, closeReason) =>
            {
                log4j.Info("client closed: " + session.SessionID + ", reason: " + closeReason.ToString());
            });
            //appServer.NewMessageReceived += new SessionHandler<JsonWebSocketSession, string>((s, message) =>
            //{
            //    log4j.Info("message: " + message);
            //});


            Console.WriteLine();

            //Try to start the appServer
            if (!appServer.Start())
            {
                Console.WriteLine("Failed to start!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("The server started successfully, press key 'q' to stop it!");

            string cmd = Console.ReadLine();

            while (cmd != "q")
            {
                

                string[] scmds = cmd.Split(' ');
                if (scmds[0] == "1") {
                    foreach (var session in appServer.GetAllSessions())
                    {
                        //session.Send("test 1");
                        if (scmds.Length >= 2 && scmds[1] != string.Empty)
                        {
                            string msg = cmd.Substring(cmd.IndexOf(" ")).Trim();
                            Data.ResponseEcho responseEcho = new Data.ResponseEcho { UUID = Guid.NewGuid().ToString(), Message = msg };
                            session.SendJsonMessage(Data.Cmd.TcsCommand.ResponseEcho.ToString(), responseEcho);
                        }
                    }
                }
                cmd = Console.ReadLine();
            }

            //Stop the appServer
            appServer.Stop();

            Console.WriteLine();
            Console.WriteLine("The server was stopped!");
        }

        static void AppServer_NewMessageReceived(WebSocketSession session, string message)
        {
            log4j.Info("message: " + message);
            //Send the received message back
            //session.Send(message);
            //foreach(var s in session.AppServer.GetAllSessions())
            //{
            //    s.Send(message);
            //}
        }

    }
}
