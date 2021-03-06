﻿using System;
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
        public string Name { get; set; }

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


            byte[] cmd1 = BitConverter.GetBytes((ushort)17408);
            byte[] dataSize = new byte[2];
            byte[] cmd3 = BitConverter.GetBytes(mainCmd);
            byte[] cmd4 = BitConverter.GetBytes(subCmd);

            byte[] sendData = cmd1.Concat(dataSize).Concat(cmd3).Concat(cmd4).Concat(datas).ToArray();
            dataSize = BitConverter.GetBytes((ushort)sendData.Length);      // dataSize is including the headerSize (8bytes)
            //log4j.Info("cmd2: " + BitConverter.ToString(cmd2));
            sendData[2] = dataSize[0];
            sendData[3] = dataSize[1];

            ArraySegment<byte> segment = new ArraySegment<byte>(sendData);
            Send(segment);

            return true;
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
