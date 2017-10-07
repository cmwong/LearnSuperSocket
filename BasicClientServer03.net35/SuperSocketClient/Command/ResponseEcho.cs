using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SuperSocket.ProtoBase;
using TcpClientServer.Data;
namespace SuperSocketClient.Command
{
    class ResponseEcho : BaseCommand
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override string Name => Cmd.TcsCommand.ResponseEcho.ToString();

        public override void ExecuteCommand(StringClient session, StringPackageInfo commandInfo)
        {
            log4j.Info("ResponseEcho: " + commandInfo.Body);

            session.PushToResponseEchoHandler(commandInfo.Body);
        }
    }
}
