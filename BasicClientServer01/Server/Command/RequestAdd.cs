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
            session.Send("ResponseAdd " + requestInfo.Parameters.Select(p => Convert.ToInt32(p)).Sum().ToString());
        }
    }
}
