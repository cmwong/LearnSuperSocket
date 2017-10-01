using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using MyClient;
using SuperSocket.ClientEngine;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Log.config", Watch = true)]
namespace testClient
{
    class Program
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static void Main(string[] args)
        {
            log4j.Info("Start Main");
            System.Net.IPEndPoint endpoint = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 2012);

            StringClient client = new StringClient();

            var connected = client.ConnectAsync(endpoint);
            Console.WriteLine("after connectAsync {0}", connected.Result);
            connected.Wait();
            Console.WriteLine("After wait connect: {0}", connected.Result);

            string cmd = "";

            while (cmd != "q")
            {
                if (cmd != "")
                {
                    //Console.WriteLine(cmd);
                    byte[] data = Encoding.UTF8.GetBytes(cmd + "\r\n");
                    //client.Send(data, 0, data.Length);
                    log4j.Info("sending command: " + cmd);
                    client.Send(data);

                }
                cmd = Console.ReadLine();
            }
            client.Close();

        }
    }
}
