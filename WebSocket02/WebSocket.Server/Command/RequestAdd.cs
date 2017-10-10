using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.WebSocket;
using SuperSocket.WebSocket.SubProtocol;
using WebSocket.Data;
using System.Threading;
using SuperSocket.WebSocket.Protocol;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;

namespace WebSocket.Server.Command
{
    public class RequestAdd : JsonSubCommandBase<JsonWebSocketSession, WebSocket.Data.RequestAdd>
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override string Name => Data.Cmd.TcsCommand.RequestAdd.ToString();

        Random random = new Random();
        protected override void ExecuteJsonCommand(JsonWebSocketSession session, Data.RequestAdd commandInfo)
        {
            log4j.Info("RequestAdd: " + Newtonsoft.Json.JsonConvert.SerializeObject(commandInfo));
            //session.Send("ResponseAdd " + requestInfo.Parameters.Select(p => Convert.ToInt32(p)).Sum().ToString());

            //////to test timeOut on client site.
            int delay = random.Next(1, 15);
            delay *= 1000;
            log4j.Info("delay: " + delay);

            Task task = Task.Factory.StartNew(() =>
            {
                ////to test timeOut on client site.
                Task.Delay(delay).Wait();

                Data.ResponseAdd responseAdd = new Data.ResponseAdd { UUID = commandInfo.UUID };
                responseAdd.Result = commandInfo.Param.Sum();
                session.SendJsonMessage(Data.Cmd.TcsCommand.ResponseAdd.ToString(), responseAdd);
            });

            // handle exception throw in task
            // https://stackoverflow.com/questions/8714235/handling-exception-with-tpl-without-wait/
            task.ContinueWith((t) =>
            {
                var ex = t.Exception;
                log4j.Info("error in task", ex);
            },
            CancellationToken.None,
            TaskContinuationOptions.OnlyOnFaulted,
            TaskScheduler.Current
            );

        }
        
    }
}
