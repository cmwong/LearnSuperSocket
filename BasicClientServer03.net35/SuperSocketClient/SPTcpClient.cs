using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ITcpClientServer;
using TcpClientServer.Data;

namespace SuperSocketClient
{
    public class SPTcpClient : ITcpEventClient
    {
        private StringClient sClient = new StringClient();
        private IPEndPoint _ipEndPoint = null;

        public IPEndPoint IPEndPoint {
            get => _ipEndPoint;
            set => throw new NotImplementedException();
        }
        private int _timeOutMilliSec = 10000;
        public int TimeOutMilliSec {
            get => _timeOutMilliSec;
            set => _timeOutMilliSec = value;
        }

        public SPTcpClient(IPEndPoint ipEndPoint)
        {
            _ipEndPoint = ipEndPoint;
            _Init();
        }
        public SPTcpClient(string serverIP, int serverPort)
        {
            _ipEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
            _Init();
        }

        private void _Init()
        {
            sClient.Closed += new EventHandler((s, e) =>
            {
                OnClosed?.Invoke(s, e);
            });
            sClient.Connected += new EventHandler((s, e) =>
            {
                OnConnected?.Invoke(s, e);
            });
            sClient.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>((s, e) =>
            {
                OnError?.Invoke(s, new ErrorEventArgs(e.Exception));
            });
            sClient.OnResponseEcho += new StringClient.ResponseEchoHandler(message =>
            {
                OnResponseEcho?.Invoke(message);
            });
        }

        public event EventHandler OnClosed;
        public event EventHandler OnConnected;
        public event EventHandler<ErrorEventArgs> OnError;
        public event ResponseEchoHandler OnResponseEcho;

        public bool IsConnected()
        {
            return sClient.IsConnected;
        }

        public void ReConnect()
        {
            sClient.Close();
            Start();
        }

        public void Start()
        {
            //Task<bool> ts = sClient.ConnectAsync(_ipEndPoint);
            //ts.Wait();
            sClient.BeginConnect(_ipEndPoint);
        }

        public void Stop()
        {
            sClient.Close();
        }


        public ResponseAdd RequestAdd(params int[] param)
        {
            return sClient.RequestAdd(_timeOutMilliSec, param);
        }

        public void RequestEcho(string message)
        {
            sClient.RequestEcho(message);
        }
    }
}
