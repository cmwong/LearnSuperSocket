using System;
using System.Collections.Generic;
using System.Text;

using SAEASocket.Custom;
using SAEA.Sockets;
using SAEA.Sockets.Interface;
using SAEA.Sockets.Handler;
using SAEA.Sockets.Core;
using System.Threading.Tasks;

namespace SAEASocket
{

    public class EventSocketServer
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected IServerSokcet _server;

        public event OnAcceptedHandler OnAccepted;
        public event Custom.OnErrorHandler OnError;
        public event Custom.OnDisconnectedHandler OnDisconnected;
        public event EventHandler<Package> OnNewPackageReceived;

        SessionIDToNumber sessionIDToNumber = new SessionIDToNumber();

        public EventSocketServer(string ipAddress, int port)
        {
            ISocketOption option = SocketOptionBuilder.Instance.UseIocp(new Context())
                .SetIP(ipAddress)
                .SetPort(port)
                .SetSocket(SAEA.Sockets.Model.SAEASocketType.Tcp)
                //.SetCount(ushort.MaxValue)
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
            log4j.Info(ut.ID);

            up.Unpack(data, (package) =>
            {
                //log4j.Info("sessionID: " + ut.ID + ", " + Newtonsoft.Json.JsonConvert.SerializeObject(package));
                OnNewPackageReceived?.Invoke(userToken, package);
            });
        }
        private void _server_OnError(string sessionID, Exception ex)
        {
            //log4j.Info(ID, ex);
            ushort index = sessionIDToNumber.Get(sessionID);

            OnError?.Invoke(sessionID, index, ex);
        }
        private void _server_OnDisconnected(string sessionID, Exception ex)
        {
            // log4j.Info(sessionID + ", " + ex.Message);
            ushort index = sessionIDToNumber.Get(sessionID);
            sessionIDToNumber.Remove(sessionID);

            OnDisconnected?.Invoke(sessionID, index, ex);
        }
        private void _server_OnAccepted(object userToken)
        {
            //IUserToken ut = (IUserToken)userToken;
            //log4j.Info(ut.ID);
            UserToken ut = (UserToken)userToken;
            try
            {
                ut.Index = sessionIDToNumber.Add(ut.ID, out bool isAdded);
                OnAccepted?.Invoke(userToken);
            }
            catch (OverflowException ex)
            {
                log4j.Info(ex.Message + ", sessionID: " + ut.ID);
                // ((SAEA.Sockets.Core.Tcp.IocpServerSocket)_server).Disconnect((IUserToken)userToken, ex);
                // (_server as SAEA.Sockets.Core.Tcp.IocpServerSocket).Disconnect(userToken as IUserToken, ex);

                // delay some time to disconnect client, cause client is still in connecting state. otherwise client not having correct connection state.
                Task.Delay(500).
                    ContinueWith(t =>
                    {
                        (_server as SAEA.Sockets.Core.Tcp.IocpServerSocket).Disconnect(userToken as IUserToken, ex);
                    });
            }
        }

        public int ClientCounts { get { return _server.ClientCounts; } }
        public IUserToken GetUserToken(string sessionID)
        {
            return (IUserToken)_server.GetCurrentObj(sessionID);
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
            try
            {
                // _server.Disconnecte(_server.GetCurrentObj(sessionID));
                _server.Disconnecte(sessionID);
            }
            catch (Exception ex)
            {
                log4j.Error(sessionID, ex);
            }
            finally
            {
                ushort index = sessionIDToNumber.Get(sessionID);
                sessionIDToNumber.Remove(sessionID);
            }
        }

        public void SendAsync(ushort index, ushort round, ushort mainCmd, ushort subCmd, byte[] datas)
        {
            string sessionID = sessionIDToNumber.Get(index);
            if (sessionID is null)
            {
                return;
            }
            _server.SendAsync(sessionID, Package.ToArray(round, mainCmd, subCmd, datas));
            
        }

        public void SendAsync(ushort index, Package package)
        {
            string sessionID = sessionIDToNumber.Get(index);
            if (sessionID is null)
            {
                log4j.Info("cannot found index: " + index);
                return;
            }
            _server.SendAsync(sessionID, package.ToArray());
        }
    }
}
