using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WebSocket.Data;
using System.IO;

namespace IClient
{
    public delegate void ResponseEchoHandler(ResponseEcho responseEcho);

    public interface IMyClient
    {
        System.Net.IPEndPoint IPEndPoint { get; set; }
        int TimeOutMilliSec { get; set; }

        void Start();
        void Stop();
        void ReConnect();

        bool IsConnected();
        event EventHandler OnClosed;
        event EventHandler OnConnected;
        event EventHandler<ErrorEventArgs> OnError;

        event ResponseEchoHandler OnResponseEcho;

        ResponseAdd RequestAdd(params int[] param);
        void RequestEcho(string message);
    }
}
