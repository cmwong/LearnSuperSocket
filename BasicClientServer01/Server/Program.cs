using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SuperSocket.SocketBase.Config;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Config/log4net.config", Watch = true)]
namespace Server
{
    class Program
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            log4j.Info("Start Main");

            StringServer appServer = new StringServer();
            ServerConfig serverConfig = new ServerConfig
            {
                Port = 2012    // the listening port
            };

            //Setup the appServer
            if (!appServer.Setup(serverConfig))
            {
                Console.WriteLine("Failed to setup!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine();

            //Try to start the appServer
            if (!appServer.Start())
            {
                Console.WriteLine("Failed to start!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("The server started successfully, press key 'q' to stop it!");

            string command = "";
            while (command != "q")
            {
                // we only hv 2 command
                // ResponseEcho text
                // ResponseAdd value
                foreach (StringSession s in appServer.GetAllSessions())
                {
                    s.Send(command);
                }
                command = Console.ReadLine();
            }

            Console.WriteLine();

            //Stop the appServer
            appServer.Stop();

            Console.WriteLine("The server was stopped!");

        }
    }
}
