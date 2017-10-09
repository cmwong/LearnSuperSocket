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
            //var appServer = new WebSocketServer();
            var appServer = new MyAppServer();

            List<SuperSocket.SocketBase.Config.ICommandAssemblyConfig> cmdConfigs = new List<ICommandAssemblyConfig>();

            //ServerConfig serverConfig = new ServerConfig
            //{
            //    Port = 2012,
            //    //CommandLoader = "RequestAdd"
            //};
            //List<SuperSocket.SocketBase.Command.ICommandLoader<
            //    SuperSocket.SocketBase.Command.ICommand<JsonWebSocketSession, SuperSocket.WebSocket.Protocol.IWebSocketFragment>>> m_commandLoader =
            //    new List<SuperSocket.SocketBase.Command.ICommandLoader<SuperSocket.SocketBase.Command.ICommand<JsonWebSocketSession, SuperSocket.WebSocket.Protocol.IWebSocketFragment>>>();

            //m_commandLoader.Add(new )

            //appServer.Setup(serverConfig, null, null, null, null,)


            //Setup the appServer
            if (!appServer.Setup(2012)) //Setup with listening port
            //if(!appServer.Setup(serverConfig))
            {
                Console.WriteLine("Failed to setup!");
                Console.ReadKey();
                return;
            }



            //appServer.NewMessageReceived += new SessionHandler<WebSocketSession, string>(appServer_NewMessageReceived);
            //appServer.NewSessionConnected += new SessionHandler<WebSocketSession>((target) =>
            //{
            //    log4j.Info("client connected: " + target.SessionID);
            //});
            appServer.NewSessionConnected += new SessionHandler<JsonWebSocketSession>((s) =>
            {
                log4j.Info("client connected: " + s.SessionID);
            });
            appServer.NewMessageReceived += new SessionHandler<JsonWebSocketSession, string>((s, message) =>
            {
                log4j.Info("message: " + message);
            });


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
                if (cmd == "1") {
                    foreach (var session in appServer.GetAllSessions())
                    {
                        session.Send("test 1");
                    }
                }
                cmd = Console.ReadLine();
            }

            //Stop the appServer
            appServer.Stop();

            Console.WriteLine();
            Console.WriteLine("The server was stopped!");
        }

        static void appServer_NewMessageReceived(WebSocketSession session, string message)
        {
            log4j.Info(message);
            //Send the received message back
            //session.Send(message);
            foreach(var s in session.AppServer.GetAllSessions())
            {
                s.Send(message);
            }
        }

    }
}
