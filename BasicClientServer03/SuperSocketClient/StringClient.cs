using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SuperSocket.ClientEngine;
using SuperSocket.ProtoBase;
using SuperSocket.ClientEngine.Protocol;
using System.Threading;
using ITcpClientServer;
using TcpClientServer.Data;

namespace SuperSocketClient
{
    class StringClient : EasyClient<StringPackageInfo>
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<string, ICommand<StringClient, StringPackageInfo>> m_CommandDict
            = new Dictionary<string, ICommand<StringClient, StringPackageInfo>>(StringComparer.OrdinalIgnoreCase);

        public StringClient() : base()
        {
            Init();
        }

        protected void Init()
        {
            Command.ResponseEcho echoCmd = new Command.ResponseEcho();
            m_CommandDict.Add(echoCmd.Name, echoCmd);
            Command.ResponseAdd addCmd = new Command.ResponseAdd();
            m_CommandDict.Add(addCmd.Name, addCmd);


            this.Error += new EventHandler<ErrorEventArgs>(Client_Error);
            this.Connected += new EventHandler((s, e) =>
            {
                log4j.Info("connected");
            });
            this.Closed += new EventHandler((s, e) =>
            {
                log4j.Info("closed");
            });

            this.Initialize(new StringReceiveFilter());
        }

        protected void Client_Error(object sender, ErrorEventArgs e)
        {
            log4j.Error(string.Format("sender: {0}", sender), e.Exception);
        }

        protected override void HandlePackage(IPackageInfo package)
        {
            StringPackageInfo cmdInfo = (StringPackageInfo)package;
            if (cmdInfo.Key.Contains("Response"))
            {
                ExceuteCommand(cmdInfo);
            }
            else
            {
                log4j.Info("unknow command: " + cmdInfo.Key + ", body: " + cmdInfo.Body);
            }
            //ExceuteCommand(cmdInfo);
        }

        protected void ExceuteCommand(StringPackageInfo cmdInfo)
        {
            //log4j.Info("key: " + cmdInfo.Key);
            if (m_CommandDict.TryGetValue(cmdInfo.Key, out ICommand<StringClient, StringPackageInfo> command))
            {
                command.ExecuteCommand(this, cmdInfo);
            }
            else
            {
                log4j.Info("unknow command: " + cmdInfo.Key + ", body: " + cmdInfo.Body);
            }
        }

        public delegate void ResponseEchoHandler(string message);
        public event ResponseEchoHandler OnResponseEcho;
        internal void PushToResponseEchoHandler(string message)
        {
            OnResponseEcho?.Invoke(message);
        }


        protected delegate void ResponseAddHandler(ResponseAdd responseAdd);
        protected event ResponseAddHandler OnResponseAdd;

        /// <summary>
        /// this will call by Command Class ResponseAdd
        /// </summary>
        /// <param name="responseAdd"></param>
        internal void PushToResponseAddHandler(ResponseAdd responseAdd)
        {
            OnResponseAdd?.Invoke(responseAdd);
        }

        public void HandlerCount()
        {
            if (OnResponseAdd != null)
            {
                log4j.Info("OnResponseAdd.count: " + OnResponseAdd.GetInvocationList().Count());
            }
            else
            {
                log4j.Info("OnResponseAdd empty");
            }
        }

        /// <summary>
        /// Send command RequestAdd to server 
        /// Receive a Data.ResponseAdd object
        /// - async method
        /// - will throw Exception "TimeOut"
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<ResponseAdd> RequestAddAsync(params int[] param)
        {
            RequestAdd requestAdd = new RequestAdd { UUID = Guid.NewGuid().ToString() };
            foreach (int p in param)
            {
                requestAdd.Param.Add(p);
            }

            // set a timeout on this call to server!
            TaskCompletionSource<ResponseAdd> tcs = new TaskCompletionSource<ResponseAdd>();
            const int timeOuts = 10000;     //miliseconds
            CancellationTokenSource ct = new CancellationTokenSource(timeOuts);

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
                tcs.TrySetException(new TimeoutException("TimeOut " + timeOuts));
            }, useSynchronizationContext: false);

            OnResponseAdd += rah;   //hook to the eventHandler
            //string sendCmd = "RequestAdd " + Newtonsoft.Json.JsonConvert.SerializeObject(requestAdd) + "\r\n";
            string sendCmd = Cmd.TcsCommand.RequestAdd.ToString() + " " + Newtonsoft.Json.JsonConvert.SerializeObject(requestAdd) + "\r\n";
            base.Send(Encoding.UTF8.GetBytes(sendCmd));

            ResponseAdd responseAdd = await tcs.Task;
            OnResponseAdd -= rah;   //after received our response, unhook it. we only expecting 1 response.

            return responseAdd;

        }

        public ResponseAdd RequestAdd(int timeOutMilliSec, params int[] param)
        {
            RequestAdd requestAdd = new RequestAdd { UUID = Guid.NewGuid().ToString() };
            foreach (int p in param)
            {
                requestAdd.Param.Add(p);
            }

            // set a timeout on this call to server!
            // if we do not hv this timeout, calling this RequestAdd method will forever waiting if server never response!
            TaskCompletionSource<ResponseAdd> tcs = new TaskCompletionSource<ResponseAdd>();
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
            string sendCmd = Cmd.TcsCommand.RequestAdd.ToString() + " " + Newtonsoft.Json.JsonConvert.SerializeObject(requestAdd) + "\r\n";
            base.Send(Encoding.UTF8.GetBytes(sendCmd));

            tcs.Task.Wait();
            ResponseAdd responseAdd = tcs.Task.Result;
            OnResponseAdd -= rah;   //after received our response, unhook it. we only expecting 1 response.

            return responseAdd;

        }

        public void RequestEcho(string message)
        {
            base.Send(Encoding.UTF8.GetBytes(Cmd.TcsCommand.RequestEcho + " " + message + "\r\n"));
        }
    }
}
