using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebSocket4Net;
using System.Threading;

namespace WebSocket.Client
{
    class MyJsonWebSocket : JsonWebSocket
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MyJsonWebSocket(string uri) : base(uri) {
            Init();
        }
        public MyJsonWebSocket(string uri, WebSocketVersion version) : base(uri, version) {
            Init();
        }
        public MyJsonWebSocket(string uri, string subProtocol) : base(uri, subProtocol) {
            Init();
        }
        public MyJsonWebSocket(string uri, List<KeyValuePair<string, string>> cookies) 
            : base(uri, string.Empty, cookies, WebSocketVersion.None) {
            Init();
        }

        public MyJsonWebSocket(string uri, string subProtocol, List<KeyValuePair<string, string>> cookies)
            : base(uri, subProtocol, cookies, WebSocketVersion.None) {
            Init();
        }
        public MyJsonWebSocket(string uri, string subProtocol, WebSocketVersion version)
            : base(uri, subProtocol, version) {
            Init();
        }

        public MyJsonWebSocket(string uri, string subProtocol, List<KeyValuePair<string, string>> cookies, WebSocketVersion version)
            : base(uri, subProtocol, cookies, null, string.Empty, string.Empty, version) {
            Init();
        }

        public MyJsonWebSocket(string uri, string subProtocol, List<KeyValuePair<string, string>> cookies, List<KeyValuePair<string, string>> customHeaderItems, string userAgent, WebSocketVersion version)
            : base(uri, subProtocol, cookies, customHeaderItems, userAgent, string.Empty, version) {
            Init();
        }

        public MyJsonWebSocket(string uri, string subProtocol, List<KeyValuePair<string, string>> cookies, List<KeyValuePair<string, string>> customHeaderItems, string userAgent, string origin, WebSocketVersion version)
            : base(uri, subProtocol, cookies, customHeaderItems, userAgent, version) {
            Init();
        }

        public MyJsonWebSocket(WebSocket4Net.WebSocket webSocket) : base(webSocket) {
            Init();
        }

        protected void Init()
        {
            On<Data.ResponseAdd>(Data.Cmd.TcsCommand.ResponseAdd.ToString(), (data) =>
            {
                // when received data ResponseAdd from server
                //log4j.Info("OnResponseAdd: " + data.Result);
                OnResponseAdd?.Invoke(data);
            });

            On<Data.ResponseEcho>(Data.Cmd.TcsCommand.ResponseEcho.ToString(), (data) =>
            {
                OnResponseEcho?.Invoke(data);
            });
        }

        protected override string SerializeObject(object target)
        {
            //log4j.Info("serialize");
            return Newtonsoft.Json.JsonConvert.SerializeObject(target);
        }
        protected override object DeserializeObject(string json, Type type)
        {
            //log4j.Info("deserialize");
            return Newtonsoft.Json.JsonConvert.DeserializeObject(json, type);
        }

        public delegate void ResponseEchoHandler(Data.ResponseEcho responseEcho);
        public event ResponseEchoHandler OnResponseEcho;

        public void RequestEcho(string message)
        {
            Data.RequestEcho requestEcho = new Data.RequestEcho {
                UUID = Guid.NewGuid().ToString(),
                Message = message
            };
            Send(Data.Cmd.TcsCommand.RequestEcho.ToString(), requestEcho);
        }


        protected delegate void ResponseAddHandler(WebSocket.Data.ResponseAdd responseAdd);
        protected event ResponseAddHandler OnResponseAdd;

        public WebSocket.Data.ResponseAdd RequestAdd(int timeOutMilliSec, params int[] param)
        {
            Data.RequestAdd requestAdd = new Data.RequestAdd { UUID = Guid.NewGuid().ToString() };
            foreach (int p in param)
            {
                requestAdd.Param.Add(p);
            }

            //Send(Data.Cmd.TcsCommand.RequestAdd.ToString(), requestAdd);

            // set a timeout on this call to server!
            // if we do not hv this timeout, calling this RequestAdd method will forever waiting if server never response!
            TaskCompletionSource<Data.ResponseAdd> tcs = new TaskCompletionSource<Data.ResponseAdd>();
            CancellationTokenSource ct = new CancellationTokenSource(timeOutMilliSec);

            ResponseAddHandler rah = ((rpAdd) =>
            {
                // only want the response UUID which is same as what we send
                if (rpAdd.UUID == requestAdd.UUID)
                {
                    tcs.TrySetResult(rpAdd);
                }
            });

            // when timeout occur, set Exception to TaskCompletionSource
            // also remove the callback from eventhandler
            ct.Token.Register(() =>
            {
                OnResponseAdd -= rah;
                tcs.TrySetException(new TimeoutException("TimeOut " + timeOutMilliSec));
            }, useSynchronizationContext: false);

            OnResponseAdd += rah;   //hook to the eventHandler
            //string sendCmd = "RequestAdd " + Newtonsoft.Json.JsonConvert.SerializeObject(requestAdd) + "\r\n";
            //string sendCmd = WebSocket.Data.Cmd.TcsCommand.RequestAdd.ToString() + " " + Newtonsoft.Json.JsonConvert.SerializeObject(requestAdd) + "\r\n";
            //base.Send(Encoding.UTF8.GetBytes(sendCmd));
            Send(WebSocket.Data.Cmd.TcsCommand.RequestAdd.ToString(), requestAdd);

            tcs.Task.Wait();
            WebSocket.Data.ResponseAdd responseAdd = tcs.Task.Result;
            OnResponseAdd -= rah;   //after received our response, unhook it. we only expecting 1 response.

            return responseAdd;
        }
    }
}
