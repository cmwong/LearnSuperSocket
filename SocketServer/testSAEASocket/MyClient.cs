using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SAEA.Sockets;
using System.Collections.Concurrent;

namespace testSAEASocket
{
    class MyClient
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        IClientSocket _client;
        MyContext _myContext;

        
        public MyClient()
        {
            _myContext = new MyContext();
            ISocketOption option = SocketOptionBuilder.Instance.UseIocp(_myContext)
                .SetIP("127.0.0.1")
                .SetPort(8800)
                .Build();

            _client = SocketFactory.CreateClientSocket(option);
            _client.OnReceive += _client_OnReceive;

        }

        private void _client_OnReceive(byte[] data)
        {
            ((MyUnpacker)_myContext.Unpacker).Unpack(data, (package) =>
            {
                log4j.Info($"{package.Body}");
            });
        }

        public void Connect()
        {
            log4j.Info("client Connecting...");
            _client.Connect();
        }
        public bool Connected { get
            {
                return _client.Connected;
            }
        }

        public void Send_1_2()
        {
            log4j.Info("Send_1_2 to server");
            string data = "this is some text 哈哈 ";
            Send(1, 2, Encoding.UTF8.GetBytes(data));
        }

        public bool Send(ushort mainCmd, ushort subCmd, byte[] datas)
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

            _client.SendAsync(sendData);
            return true;
        }
    }
}
