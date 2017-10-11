using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SuperSocket.WebSocket;
using SuperSocket.WebSocket.SubProtocol;
using WebSocket.Data;

namespace WebSocket.Server.Command
{
    public class RequestEcho : JsonSubCommandBase<JsonWebSocketSession, WebSocket.Data.RequestEcho>
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override string Name => Data.Cmd.TcsCommand.RequestEcho.ToString();

        protected override void ExecuteJsonCommand(JsonWebSocketSession session, Data.RequestEcho commandInfo)
        {
            log4j.Info("RequestEcho: " + Newtonsoft.Json.JsonConvert.SerializeObject(commandInfo));
            Data.ResponseEcho responseEcho = new Data.ResponseEcho {
                UUID = commandInfo.UUID,
                Message = commandInfo.Message
            };
            session.SendJsonMessage(Data.Cmd.TcsCommand.ResponseEcho.ToString(), responseEcho);
        }
    }
}
