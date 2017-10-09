using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.WebSocket;
using SuperSocket.WebSocket.SubProtocol;
using WebSocket.Data;
using System.Threading;

namespace WebSocket.Server.Command
{
    public class RequestAdd : JsonSubCommandBase<MyAppSession, SubRequestInfo>
    //public class RequestAdd : JsonSubCommand<WebSocket.Data.RequestAdd>
    //public class RequestAdd : SubCommandBase<MyAppSession>
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override string Name => Data.Cmd.TcsCommand.RequestAdd.ToString();

        Random random = new Random();
        //protected override void ExecuteJsonCommand(WebSocketSession session, Data.RequestAdd commandInfo)
        //{
        //    log4j.Info("RequestAdd: " + Newtonsoft.Json.JsonConvert.SerializeObject(commandInfo));
        //    //session.Send("ResponseAdd " + requestInfo.Parameters.Select(p => Convert.ToInt32(p)).Sum().ToString());

        //    //////to test timeOut on client site.
        //    int delay = random.Next(1, 15);
        //    delay *= 1000;
        //    log4j.Info("delay: " + delay);

        //    Task task = Task.Factory.StartNew(() =>
        //    {
        //        ////to test timeOut on client site.
        //        Task.Delay(delay).Wait();

        //        Data.ResponseAdd responseAdd = new Data.ResponseAdd { UUID = commandInfo.UUID };
        //        responseAdd.Result = commandInfo.Param.Sum();
        //        //session.Send("ResponseAdd " + Newtonsoft.Json.JsonConvert.SerializeObject(responseAdd));
        //        //session.Send(Data.Cmd.TcsCommand.ResponseAdd.ToString() + " " + Newtonsoft.Json.JsonConvert.SerializeObject(responseAdd));

        //        SendJsonMessage(session, responseAdd);
        //        //throw new Exception("wa haha!");
        //    });

        //    // handle exception throw in task
        //    // https://stackoverflow.com/questions/8714235/handling-exception-with-tpl-without-wait/
        //    task.ContinueWith((t) =>
        //    {
        //        var ex = t.Exception;
        //        log4j.Info("error in task", ex);
        //    },
        //    CancellationToken.None,
        //    TaskContinuationOptions.OnlyOnFaulted,
        //    TaskScheduler.Current
        //    );

        //}

        //public override void ExecuteCommand(MyAppSession session, SubRequestInfo requestInfo)
        //{
        //    log4j.Info("RequestAdd: " + requestInfo.Body);
        //    //session.Send("ResponseAdd " + requestInfo.Parameters.Select(p => Convert.ToInt32(p)).Sum().ToString());

        //    //////to test timeOut on client site.
        //    int delay = random.Next(1, 15);
        //    delay *= 1000;
        //    log4j.Info("delay: " + delay);

        //    Task task = Task.Factory.StartNew(() =>
        //    {
        //        ////to test timeOut on client site.
        //        Task.Delay(delay).Wait();

        //        Data.RequestAdd requestAdd = Newtonsoft.Json.JsonConvert.DeserializeObject<Data.RequestAdd>(requestInfo.Body);
        //        Data.ResponseAdd responseAdd = new Data.ResponseAdd { UUID = requestAdd.UUID };
        //        responseAdd.Result = requestAdd.Param.Sum();
        //        //session.Send("ResponseAdd " + Newtonsoft.Json.JsonConvert.SerializeObject(responseAdd));
        //        session.Send(Data.Cmd.TcsCommand.ResponseAdd.ToString() + " " + Newtonsoft.Json.JsonConvert.SerializeObject(responseAdd));

        //        //throw new Exception("wa haha!");
        //    });

        //    // handle exception throw in task
        //    // https://stackoverflow.com/questions/8714235/handling-exception-with-tpl-without-wait/
        //    task.ContinueWith((t) =>
        //    {
        //        var ex = t.Exception;
        //        log4j.Info("error in task", ex);
        //    },
        //    CancellationToken.None,
        //    TaskContinuationOptions.OnlyOnFaulted,
        //    TaskScheduler.Current
        //    );
        //}

        protected override void ExecuteJsonCommand(MyAppSession session, SubRequestInfo commandInfo)
        {
            log4j.Info("RequestAdd: " + commandInfo.Body);
            //session.Send("ResponseAdd " + requestInfo.Parameters.Select(p => Convert.ToInt32(p)).Sum().ToString());

            //////to test timeOut on client site.
            int delay = random.Next(1, 15);
            delay *= 1000;
            log4j.Info("delay: " + delay);

            Task task = Task.Factory.StartNew(() =>
            {
                ////to test timeOut on client site.
                Task.Delay(delay).Wait();

                Data.RequestAdd requestAdd = Newtonsoft.Json.JsonConvert.DeserializeObject<Data.RequestAdd>(commandInfo.Body);
                Data.ResponseAdd responseAdd = new Data.ResponseAdd { UUID = requestAdd.UUID };
                responseAdd.Result = requestAdd.Param.Sum();
                //session.Send("ResponseAdd " + Newtonsoft.Json.JsonConvert.SerializeObject(responseAdd));
                session.Send(Data.Cmd.TcsCommand.ResponseAdd.ToString() + " " + Newtonsoft.Json.JsonConvert.SerializeObject(responseAdd));

                //throw new Exception("wa haha!");
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
