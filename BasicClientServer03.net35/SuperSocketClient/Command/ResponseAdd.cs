using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SuperSocket.ProtoBase;
using TcpClientServer.Data;
namespace SuperSocketClient.Command
{
    class ResponseAdd : BaseCommand
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override string Name => Cmd.TcsCommand.ResponseAdd.ToString();

        public override void ExecuteCommand(StringClient session, StringPackageInfo commandInfo)
        {
            log4j.Debug("ResponseAdd: " + commandInfo.Body);
            try
            {
                // after received data, pass the data to EventHandler
                TcpClientServer.Data.ResponseAdd responseAdd = Newtonsoft.Json.JsonConvert.DeserializeObject<TcpClientServer.Data.ResponseAdd>(commandInfo.Body);
                session.PushToResponseAddHandler(responseAdd);
            }
            catch (Exception ex)
            {
                log4j.Error("", ex);
            }
        }
    }
}
