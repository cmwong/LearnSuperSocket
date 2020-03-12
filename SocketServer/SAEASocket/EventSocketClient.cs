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

        protected IClientSocket _client;
        protected Context _context;

        public event EventHandler<Package> OnNewPackageReceived;
        public event SAEA.Sockets.Handler.OnErrorHandler OnError;
        public event SAEA.Sockets.Handler.OnDisconnectedHandler OnDisconnected;
        public event OnConnectedHandler OnConnected;

        private System.Timers.Timer sendAliveTimer;

        public bool IsConnected { get { return _client.Connected; } }

        public EventSocketClient(string ipAddress, int port)
        {
            _context = new Context();
            ISocketOption option = SocketOptionBuilder.Instance.UseIocp(_context)
                .SetIP(ipAddress)
                .SetPort(port)
                .Build();

            _client = SocketFactory.CreateClientSocket(option);
            // _client = new Core.Tcp.IocpClientSocket(option);
            _client.OnReceive += _client_OnReceive;
            _client.OnError += _client_OnError;
            _client.OnDisconnected += _client_OnDisconnected;

            sendAliveTimer = new System.Timers.Timer(30000);
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
                        _client_OnConnected();
                        break;
                    default:
                        OnDisconnected?.Invoke("", new SocketException((int)e));
                        break;
                }
            });

        }
        //private void ExceptionHandler(Task task)
        //{
        //    log4j.Info("ExceptionHandler", task.Exception);
        //}

        public void Connect()
        {
            _client.Connect();
        }

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
            ((Unpacker)_context.Unpacker).Unpack(data, (package) =>
            {
                OnNewPackageReceived?.Invoke(this, package);
            });
        }

        private void _client_OnConnected()
        {
            OnConnected?.Invoke();
            // sendAliveTimer.Start();
        }

        private void SendAliveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            long tick = DateTime.Now.Ticks;
            //byte[] datas = BitConverter.GetBytes(tick);
            byte[] datas = Encoding.UTF8.GetBytes(tick.ToString());
            try
            {
                log4j.Info("sendAlive: " + tick);
                SendAsync(0, 1, datas);
            }
            catch (Exception ex)
            {
                log4j.Error("Send error", ex);
            }
        }

        public bool Send(ushort mainCmd, ushort subCmd, byte[] datas)
        {
            bool val = false;
            if (!IsConnected)
            {
                return val;
            }

            //byte[] sendData = MessageToByteArray(mainCmd, subCmd, datas);
            byte[] sendData = Package.ToArray(17408, mainCmd, subCmd, datas);

            _client.Send(sendData);
            return true;
        }
        public bool Send(Package package)
        {
            bool val = false;
            if (!IsConnected)
            {
                return val;
            }

            _client.Send(package.ToArray());
            return true;
        }

        public void SendAsync(ushort mainCmd, ushort subCmd, byte[] datas)
        {
            _client.SendAsync(Package.ToArray(17408, mainCmd, subCmd, datas));
        }

        public void SendAsync(Package package)
        {
            _client.SendAsync(package.ToArray());
        }
    }
}
