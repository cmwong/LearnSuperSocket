using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
namespace Server.Command
{
    public class RequestAdd : CommandBase<StringSession, StringRequestInfo>
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void ExecuteCommand(StringSession session, StringRequestInfo requestInfo)
        {
            log4j.Info("RequestAdd: " + requestInfo.Body);
            //session.Send("ResponseAdd " + requestInfo.Parameters.Select(p => Convert.ToInt32(p)).Sum().ToString());

            ////to test timeOut on client site.
            //int delay = new Random().Next(5, 15);
            //delay *= 1000;
            //Task.Delay(delay).Wait();
            ////Task.Delay(20000).Wait();

            Data.RequestAdd requestAdd = Newtonsoft.Json.JsonConvert.DeserializeObject<Data.RequestAdd>(requestInfo.Body);

            Data.ResponseAdd responseAdd = new Data.ResponseAdd { UUID = requestAdd.UUID };
            responseAdd.Result = requestAdd.Param.Sum();
            session.Send("ResponseAdd " + Newtonsoft.Json.JsonConvert.SerializeObject(responseAdd));
        }
    }
}
