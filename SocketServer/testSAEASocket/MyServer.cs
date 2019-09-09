using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SAEA.Sockets;
using SAEA.Sockets.Interface;

namespace testSAEASocket
{
    class MyServer
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        IServerSokcet _server;
        public event Action<MyPackage> OnReceived;

        public MyServer()
        {
            var option = SocketOptionBuilder.Instance.UseIocp(new MyContext())
                .SetPort(8800)
                .Build();

            _server = SocketFactory.CreateServerSocket(option);
            _server.OnAccepted += _server_OnAccepted;
            _server.OnReceive += _server_OnReceive;
        }

        private void _server_OnReceive(object userToken, byte[] data)
        {
            IUserToken ut = (IUserToken)userToken;
            MyUnpacker up = (MyUnpacker)ut.Unpacker;
            // log4j.Info("message length: " + BitConverter.ToUInt16(data, 2));
            // string txt = Encoding.UTF8.GetString(data);
            // log4j.Info(ut.ID + ", data: " + txt);

            up.Unpack(data, (package) =>
            {
                log4j.Info(Newtonsoft.Json.JsonConvert.SerializeObject(package));
                OnReceived?.Invoke(package);
            });
        }

        private void _server_OnAccepted(object obj)
        {
            IUserToken userToken = (IUserToken)obj;

            log4j.Info(userToken.ID);

            // Send(userToken.ID, 2, 2, Encoding.UTF8.GetBytes("helo"));
            TestSendAlot(userToken.ID);
        }

        public void Start()
        {
            _server.Start();
        }

        public bool Send(string sessionID, ushort mainCmd, ushort subCmd, byte[] datas)
        {
            //bool val = false;
            //if (!Connected)
            //    return val;

            byte[] cmd1 = BitConverter.GetBytes((ushort)17408);
            byte[] dataSize = new byte[2];
            byte[] cmd3 = BitConverter.GetBytes(mainCmd);
            byte[] cmd4 = BitConverter.GetBytes(subCmd);

            //log4j.Info("cmd1: " + BitConverter.ToString(cmd1));
            //log4j.Info("cmd3: " + BitConverter.ToString(cmd3));
            //log4j.Info("cmd4: " + BitConverter.ToString(cmd4));

            byte[] sendData = cmd1.Concat(dataSize).Concat(cmd3).Concat(cmd4).Concat(datas).ToArray();
            dataSize = BitConverter.GetBytes((ushort)sendData.Length);
            sendData[2] = dataSize[0];
            sendData[3] = dataSize[1];

            //if (datas.Length > AppServer.Config.MaxRequestLength)
            //{
            //    log4j.Debug("data too long");
            //    return val;
            //}

            //ArraySegment<byte> segment = new ArraySegment<byte>(sendData);
            //Send(segment);
            //val = true;
            //return val;

            //return TrySend(segment);

            _server.SendAsync(sessionID, sendData);
            return true;
        }


        public void TestSendAlot(string sessionID)
        {
            ushort key = 1;
            ushort subKey = 255;
            string data = "this is some text 哈哈 ";
            int count = 6000;

            log4j.Info("TestSendAlot " + count);
            for (int i = 0; i < count; i++)
            {
                Send(sessionID, key, subKey, Encoding.UTF8.GetBytes(data + ": " + i));
            }
            log4j.Info("finish");

        }
    }
}
