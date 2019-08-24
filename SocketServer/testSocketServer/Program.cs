using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testSocketServer
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketServer.EventServer appServer = new SocketServer.EventServer();
            if(!appServer.Setup(8800))
            {
                Console.WriteLine("Failed to setup");
                return;
            }
            if(!appServer.Start())
            {
                Console.WriteLine("Failed to start");
                return;
            }
            Console.WriteLine("server start successfully, press key 'q' to stop it!");
            while(Console.ReadKey().KeyChar != 'q')
            {
                Console.WriteLine();
                continue;
            }

            appServer.Stop();
            Console.WriteLine("server stoped");
        }
    }
}
