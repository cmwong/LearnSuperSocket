using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SAEA.Sockets;
using SAEA.Sockets.Handler;
using SAEASocket.Custom;
using SAEA.Sockets.Core.Tcp;

namespace SAEASocket
{
    public class EventSocketClient
    {
        IClientSocket _client;
        Context _myContext;

        public event EventHandler<Package> NewPackageReceived;

        public EventSocketClient(string ipAddress, int port)
        {
            ISocketOption option = SocketOptionBuilder.Instance.UseIocp(_myContext)
                .SetIP(ipAddress)
                .SetPort(port)
                .Build();

            _client = SocketFactory.CreateClientSocket(option);
            _client.OnReceive += _client_OnReceive;
            _client.OnError += _client_OnError;
            _client.OnDisconnected += _client_OnDisconnected;
        }

        public void Connect()
        {
            _client.ConnectAsync();
        }

        private void _client_OnDisconnected(string ID, Exception ex)
        {
            throw new NotImplementedException();
        }

        private void _client_OnError(string ID, Exception ex)
        {
            throw new NotImplementedException();
        }

        private void _client_OnReceive(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
