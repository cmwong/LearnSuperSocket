using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System.Threading;

namespace Server.Command
{
    public class RequestAdd : CommandBase<StringSession, StringRequestInfo>
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        Random random = new Random();

        public override void ExecuteCommand(StringSession session, StringRequestInfo requestInfo)
        {

            log4j.Info("RequestAdd: " + requestInfo.Body);
            //session.Send("ResponseAdd " + requestInfo.Parameters.Select(p => Convert.ToInt32(p)).Sum().ToString());

            //////to test timeOut on client site.
            int delay = random.Next(5, 15);
            delay *= 1000;
            log4j.Info("delay: " + delay);

            // 20171004
            // if code is sync and is blocking, server is processing the request 1 by 1
            // since we r making our client calling having timeOut, if 1 call is blocking, all the following call will be timeOut.
            //Task.Delay(delay).Wait();
            //Data.RequestAdd requestAdd = Newtonsoft.Json.JsonConvert.DeserializeObject<Data.RequestAdd>(requestInfo.Body);
            //Data.ResponseAdd responseAdd = new Data.ResponseAdd { UUID = requestAdd.UUID };
            //responseAdd.Result = requestAdd.Param.Sum();
            //session.Send("ResponseAdd " + Newtonsoft.Json.JsonConvert.SerializeObject(responseAdd));
            // end 20171004

            // change to let the server process in Task!
            Task task = Task.Factory.StartNew(() =>
            {
                ////to test timeOut on client site.
                Task.Delay(delay).Wait();

                Data.RequestAdd requestAdd = Newtonsoft.Json.JsonConvert.DeserializeObject<Data.RequestAdd>(requestInfo.Body);
                Data.ResponseAdd responseAdd = new Data.ResponseAdd { UUID = requestAdd.UUID };
                responseAdd.Result = requestAdd.Param.Sum();
                session.Send("ResponseAdd " + Newtonsoft.Json.JsonConvert.SerializeObject(responseAdd));
                throw new Exception("wa haha!");
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
