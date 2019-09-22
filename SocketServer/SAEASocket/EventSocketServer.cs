﻿using System;
using System.Collections.Generic;
using System.Text;

using SAEASocket.Custom;
using SAEA.Sockets;
using SAEA.Sockets.Interface;
using SAEA.Sockets.Handler;
using SAEA.Sockets.Core;

namespace SAEASocket
{
    public class EventSocketServer
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected IServerSokcet _server;

        public event OnAcceptedHandler OnAccepted;
        public event OnErrorHandler OnError;
        public event OnDisconnectedHandler OnDisconnected;
        public event EventHandler<Package> OnNewPackageReceived;

        public EventSocketServer(string ipAddress, int port)
        {
            ISocketOption option = SocketOptionBuilder.Instance.UseIocp(new Context())
                .SetIP(ipAddress)
                .SetPort(port)
                .Build();

            _server = SocketFactory.CreateServerSocket(option);
            _server.OnAccepted += _server_OnAccepted;
            _server.OnDisconnected += _server_OnDisconnected;
            _server.OnError += _server_OnError;
            _server.OnReceive += _server_OnReceive;
        }

        private void _server_OnReceive(object userToken, byte[] data)
        {
            IUserToken ut = (IUserToken)userToken;
            Unpacker up = (Unpacker)ut.Unpacker;

            up.Unpack(data, (package) =>
            {
                log4j.Info("sessionID: " + ut.ID + ", " + Newtonsoft.Json.JsonConvert.SerializeObject(package));
                OnNewPackageReceived?.Invoke(userToken, package);
            });
        }
        private void _server_OnError(string ID, Exception ex)
        {
            OnError?.Invoke(ID, ex);
        }
        private void _server_OnDisconnected(string ID, Exception ex)
        {
            OnDisconnected?.Invoke(ID, ex);
        }
        private void _server_OnAccepted(object userToken)
        {
            OnAccepted?.Invoke(userToken);
        }

        public int ClientCounts { get { return _server.ClientCounts; } }
        public IUserToken GetUserToken(string sessionID)
        {
            return (IUserToken) _server.GetCurrentObj(sessionID);
        }
        public void Start()
        {
            _server.Start();
        }
        public void Stop()
        {
            _server.Stop();
        }
        public void Disconnect(string sessionID)
        {
            _server.Disconnecte(sessionID);
        }
    }
}
