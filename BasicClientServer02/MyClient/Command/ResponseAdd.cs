using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SuperSocket.ProtoBase;
namespace MyClient.Command
{
    public class ResponseAdd : CommandBase
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void ExecuteCommand(StringClient session, StringPackageInfo commandInfo)
        {
            log4j.Debug("ResponseAdd: " + commandInfo.Body);

            // after received data, pass the data to EventHandler
            Data.ResponseAdd responseAdd = Newtonsoft.Json.JsonConvert.DeserializeObject<Data.ResponseAdd>(commandInfo.Body);
            session.PushToResponseAddHandler(responseAdd);
        }
    }
}
