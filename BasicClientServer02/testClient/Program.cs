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

            // test Command RequestAdd 
            //BatchSendRequestAdd(client);

            string cmd = "";
            while (cmd != "q")
            {
                if (cmd == "1")
                {
                    int a = 4;
                    int b = 4;
                    try
                    {
                        //Task<Data.ResponseAdd> response = client.RequestAddAsync(a, b);
                        //log4j.Info(string.Format("responseAdd: {0} + {1} = {2}", a, b, response.Result.Result));
                        Task t1 = Task.Factory.StartNew(() =>
                        {
                            Data.ResponseAdd response = client.RequestAdd(a, b);
                            log4j.Info(string.Format("responseAdd: {0} + {1} = {2}", a, b, response.Result));
                        });
                        //client.Send(Encoding.UTF8.GetBytes("RequestEcho abcdefg\r\n"));
                        client.Send(Encoding.UTF8.GetBytes(Data.Cmd.MyCommand.RequestEcho.ToString() + " abcdefg\r\n"));
                        t1.Wait();
                    }
                    catch (TimeoutException ex)
                    {
                        log4j.Info("TimeOut", ex);
                    }
                    catch (Exception ex)
                    {
                        log4j.Info("error", ex);
                    }
                }
                else if (cmd == "2")
                {
                    //just to check and see did we remove the callback correctly!
                    client.HandlerCount();
                }
                else if(cmd == "3")
                {
                    BatchSendRequestAdd(client);
                }
                //else if (cmd != "")
                //{
                //    // Server Command
                //    // RequestEcho text
                //    byte[] data = Encoding.UTF8.GetBytes(cmd + "\r\n");
                //    //client.Send(data, 0, data.Length);
                //    log4j.Info("sending raw command: " + cmd);
                //    client.Send(data);

                //}
                cmd = Console.ReadLine();
            }
            client.Close();

        }

        private static void BatchSendRequestAdd(StringClient client)
        {
            int v1 = new Random().Next(3, 9);
            int[] v2 = new int[] { 1, 2, 3, 4, 5 };
            foreach (int i in v2)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        log4j.Info(string.Format("RequestAdd {0} {1}", v1, i));
                        //Task<Data.ResponseAdd> responseAdd = client.RequestAddAsync(v1, i);
                        //log4j.Info(string.Format("ResponseAdd {0} + {1} = {2}", v1, i, responseAdd.Result.Result));

                        Data.ResponseAdd response = client.RequestAdd(v1, i);
                        log4j.Info(string.Format("responseAdd: {0} + {1} = {2}", v1, i, response.Result));

                    }
                    catch (Exception ex)
                    {
                        log4j.Info(string.Format("RequestAdd {0} {1} {2}", v1, i, ex.Message));
                    }
                });
            }
        }
    }
}
