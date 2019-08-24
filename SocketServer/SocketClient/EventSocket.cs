using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Net;

namespace SocketClient
{
    public class EventSocket : SuperSocket.ClientEngine.EasyClient<PackageInfo.EventPackageInfo>
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string ServerIP { get; protected set; }
        public int ServerPort { get; protected set; }

        private System.Timers.Timer sendAliveTimer;

        public EventSocket(string serverIP, int serverPort)
        {
            ServerIP = serverIP;
            ServerPort = serverPort;
            Init();
        }
        private void Init()
        {
            Initialize(new Filter.EventSocketFixedHeaderReceiveFilter());

            Connected += EventSocket_Connected;
            Closed += EventSocket_Closed;
            Error += EventSocket_Error;

            sendAliveTimer = new System.Timers.Timer(15000);
            sendAliveTimer.Elapsed += SendAliveTimer_Elapsed;
            sendAliveTimer.AutoReset = true;

        }

        private void EventSocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            sendAliveTimer.Stop();
        }

        private void EventSocket_Closed(object sender, EventArgs e)
        {
            sendAliveTimer.Stop();
        }

        private void SendAliveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            long tick = DateTime.Now.Ticks;
            //byte[] datas = BitConverter.GetBytes(tick);
            byte[] datas = Encoding.UTF8.GetBytes(tick.ToString());
            try
            {
                //log4j.Info("sendAlive: " + tick);
                Send(0, 1, datas);
            }
            catch (Exception ex)
            {
                log4j.Error("Send error", ex);
            }
        }
        private void EventSocket_Connected(object sender, EventArgs e)
        {
            // after connected, start a thread every 20sec send a keep alive to server.
            log4j.Info("Socket Connected");
            //Thread th = new Thread(new ThreadStart(SendAlive));
            //th.Start();
            sendAliveTimer.Start();
        }
        //protected void SendAlive()
        //{
        //    // send a time Ticks to the server
        //    while (IsConnected)
        //    {
        //        long tick = DateTime.Now.Ticks;
        //        //byte[] datas = BitConverter.GetBytes(tick);
        //        byte[] datas = Encoding.UTF8.GetBytes(tick.ToString());
        //        try
        //        {
        //            Send(0, 1, datas);
        //        }
        //        catch (Exception ex)
        //        {
        //            log4j.Error("Send error", ex);
        //        }
        //        Thread.Sleep(20000);
        //    }
        //}
        public bool Send(ushort mainCmd, ushort subCmd, string dataText)
        {
            byte[] datas = Encoding.UTF8.GetBytes(dataText);

            return Send(mainCmd, subCmd, datas);
        }
        public bool Send(ushort mainCmd, ushort subCmd, byte[] datas)
        {
            bool val = false;
            if (!IsConnected)
                return val;
            if (datas.Length > 2000)
            {
                log4j.Debug("data too long");
                return val;
            }

            byte[] cmd1 = BitConverter.GetBytes((ushort)17408);
            byte[] cmd2 = new byte[2];
            byte[] cmd3 = BitConverter.GetBytes(mainCmd);
            byte[] cmd4 = BitConverter.GetBytes(subCmd);

            //log4j.Info("cmd1: " + BitConverter.ToString(cmd1));
            //log4j.Info("cmd3: " + BitConverter.ToString(cmd3));
            //log4j.Info("cmd4: " + BitConverter.ToString(cmd4));

            byte[] sendData = cmd1.Concat(cmd2).Concat(cmd3).Concat(cmd4).Concat(datas).ToArray();
            cmd2 = BitConverter.GetBytes((ushort)sendData.Length);
            //log4j.Info("cmd2: " + BitConverter.ToString(cmd2));
            sendData[2] = cmd2[0];
            sendData[3] = cmd2[1];

            ArraySegment<byte> segment = new ArraySegment<byte>(sendData);
            Send(segment);
            val = true;

            return val;
        }
        //public bool Connect()
        //{
        //    EndPoint server = new IPEndPoint(IPAddress.Parse(ServerIP), ServerPort);

        //    //Task<bool> ts = ConnectAsync(server);
        //    //ts.Wait();
        //    //bool val = ts.Result;
        //    BeginConnect(server);

        //    return true;
        //}
        public async Task<bool> Connect()
        {
            EndPoint server = new IPEndPoint(IPAddress.Parse(ServerIP), ServerPort);

            return await ConnectAsync(server);
        }
    }
}
