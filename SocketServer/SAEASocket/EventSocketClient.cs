using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SAEA.Sockets;
using SAEA.Sockets.Handler;
using SAEASocket.Custom;
// using SAEA.Sockets.Core.Tcp;
using System.Text;
using System.Linq;

namespace SAEASocket
{
    public class EventSocketClient
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        IClientSocket _client;
        Context _myContext;

        public event EventHandler<Package> OnNewPackageReceived;
        public event OnErrorHandler OnError;
        public event OnDisconnectedHandler OnDisconnected;
        public event OnConnectedHandler OnConnected;

        private System.Timers.Timer sendAliveTimer;

        public bool Connected { get { return _client.Connected; } }

        public EventSocketClient(string ipAddress, int port)
        {
            _myContext = new Context();
            ISocketOption option = SocketOptionBuilder.Instance.UseIocp(_myContext)
                .SetIP(ipAddress)
                .SetPort(port)
                .Build();

            // _client = SocketFactory.CreateClientSocket(option);
            _client = new Client(option);
            _client.OnReceive += _client_OnReceive;
            _client.OnError += _client_OnError;
            _client.OnDisconnected += _client_OnDisconnected;

            sendAliveTimer = new System.Timers.Timer(15000);
            sendAliveTimer.Elapsed += SendAliveTimer_Elapsed;
            sendAliveTimer.AutoReset = true;

        }

        public void ConnectAsync()
        {
            _client.ConnectAsync((e) =>
            {
                log4j.Info("in callback of SocketError, " + e);
                switch (e)
                {
                    case SocketError.Success:
                        OnConnected?.Invoke();
                        sendAliveTimer.Start();
                        break;
                    default:
                        OnDisconnected?.Invoke("", new SocketException((int)e));
                        break;
                }
            });
        }
        //public void Connect()
        //{
        //    _client.Connect();
        //    if (_client.Connected)
        //    {
        //        OnConnected?.Invoke();
        //        sendAliveTimer.Start();
        //    }
        //    else
        //    {

        //    }
        //}

        private void _client_OnDisconnected(string ID, Exception ex)
        {
            log4j.Info("OnDisconnected", ex);
            OnDisconnected?.Invoke(ID, ex);
            sendAliveTimer.Stop();
        }

        private void _client_OnError(string ID, Exception ex)
        {
            log4j.Info("OnError", ex);
            OnError?.Invoke(ID, ex);
        }

        private void _client_OnReceive(byte[] data)
        {
            ((Unpacker)_myContext.Unpacker).Unpack(data, (package) =>
            {
                OnNewPackageReceived?.Invoke(this, package);
            });
        }

        private void SendAliveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            long tick = DateTime.Now.Ticks;
            //byte[] datas = BitConverter.GetBytes(tick);
            byte[] datas = Encoding.UTF8.GetBytes(tick.ToString());
            try
            {
                log4j.Info("sendAlive: " + tick);
                Send(0, 1, datas);
            }
            catch (Exception ex)
            {
                log4j.Error("Send error", ex);
            }
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

            _client.SendAsync(sendData);
            return true;
        }

        public bool Send(Package package)
        {
            return Send(package.MainKey, package.SubKey, Encoding.UTF8.GetBytes(package.Body));
        }
    }
}
