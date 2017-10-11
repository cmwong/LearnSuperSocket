using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using IClient;
using WebSocket.Data;

namespace WebSocket.Client
{
    public class MyWebSocketClient : IClient.IMyClient
    {
        private MyJsonWebSocket sClient = null;
        private IPEndPoint _ipEndPoint = null;

        public IPEndPoint IPEndPoint
        {
            get => _ipEndPoint;
            set => throw new NotImplementedException();
        }
        private int _timeOutMilliSec = 10000;
        public int TimeOutMilliSec
        {
            get => _timeOutMilliSec;
            set => _timeOutMilliSec = value;
        }

        public MyWebSocketClient(IPEndPoint ipEndPoint)
        {
            _ipEndPoint = ipEndPoint;
            _Init();
        }
        public MyWebSocketClient(string serverIP, int serverPort)
        {
            _ipEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
            _Init();
        }

        private void _Init()
        {
            sClient = new WebSocket.Client.MyJsonWebSocket(string.Format("ws://{0}:{1}", IPEndPoint.Address.ToString(), IPEndPoint.Port));

            sClient.Closed += new EventHandler((s, e) =>
            {
                OnClosed?.Invoke(s, e);
            });
            sClient.Opened += new EventHandler((s, e) =>
            {
                OnConnected?.Invoke(s, e);
            });
            sClient.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>((s, e) =>
            {
                OnError?.Invoke(s, new ErrorEventArgs(e.Exception));
            });
            sClient.OnResponseEcho += new MyJsonWebSocket.ResponseEchoHandler(responseEcho =>
            {
                OnResponseEcho?.Invoke(responseEcho);
            });
        }

        public event EventHandler OnClosed;
        public event EventHandler OnConnected;
        public event EventHandler<ErrorEventArgs> OnError;
        public event ResponseEchoHandler OnResponseEcho;

        public bool IsConnected()
        {
            return sClient.State == WebSocket4Net.WebSocketState.Open;
        }

        public void ReConnect()
        {
            sClient.Close();
            Start();
        }

        public void Start()
        {
            sClient.Open();
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

        public void HandlerCount()
        {
            sClient.HandlerCount();
        }

    }
}
