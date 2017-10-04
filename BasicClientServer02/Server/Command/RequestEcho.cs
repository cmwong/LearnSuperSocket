using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
namespace Server.Command
{
    public class RequestEcho : CommandBase<StringSession, StringRequestInfo>
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override string Name => Data.Cmd.MyCommand.RequestEcho.ToString();

        public override void ExecuteCommand(StringSession session, StringRequestInfo requestInfo)
        {
            session.Send("ResponseEcho " + requestInfo.Body);
            log4j.Info("server send ECHO: " + requestInfo.Body);
        }
    }
}
