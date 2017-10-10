using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebSocket4Net;
using WebSocket4Net.Command;
using WebSocket.Data;

namespace WebSocket.Client.Command
{
    public class ResponseAdd : WebSocketCommandBase
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override string Name => Cmd.TcsCommand.ResponseAdd.ToString();

        public override void ExecuteCommand(WebSocket4Net.WebSocket session, WebSocketCommandInfo commandInfo)
        {
            log4j.Debug("ResponseAdd: " + commandInfo.Key);

        }
    }
}
