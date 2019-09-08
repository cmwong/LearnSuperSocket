using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SAEA.Sockets;
using SAEA.Sockets.Handler;
using SAEASocket.Custom;
// using SAEA.Sockets.Core.Tcp;

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

        public bool Connected { get { return _client.Connected; } }

        public EventSocketClient(string ipAddress, int port)
        {
            _myContext = new Context();
            ISocketOption option = SocketOptionBuilder.Instance.UseIocp(_myContext)
                .SetIP(ipAddress)
                .SetPort(port)
                .Build();

            // _client = SocketFactory.CreateClientSocket(option);
            _client = new Custom.Client(option);
            _client.OnReceive += _client_OnReceive;
            _client.OnError += _client_OnError;
            _client.OnDisconnected += _client_OnDisconnected;
        }

        public void ConnectAsync()
        {
            _client.ConnectAsync((e) => {
                log4j.Info("in callback of SocketError, " + e);
                switch(e)
                {
                    case SocketError.Success:
                        OnConnected?.Invoke();
                        break;
                    default:
                        OnError?.Invoke("", new SocketException((int)e));
                        break;
                }
            });
        }
        public void Connect()
        {
            _client.Connect();
            if(_client.Connected)
            {
                OnConnected?.Invoke();
            }
        }

        private void _client_OnDisconnected(string ID, Exception ex)
        {
            log4j.Info("OnDisconnected", ex);
            OnDisconnected?.Invoke(ID, ex);
        }

        private void _client_OnError(string ID, Exception ex)
        {
            log4j.Info("OnError", ex);
            OnError?.Invoke(ID, ex);
        }

        private void _client_OnReceive(byte[] data)
        {
            log4j.Info("OnReceive");
            ((Unpacker)_myContext.Unpacker).Unpack(data, (package) =>
            {
                OnNewPackageReceived?.Invoke(this, package);
            });
        }
    }
}
