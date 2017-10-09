using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SuperSocket.WebSocket;

namespace WebSocket.Server
{
    public class MyAppServer : WebSocketServer<JsonWebSocketSession>
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // override to use Newtonsoft.Json
        public override string JsonSerialize(object target)
        {
            log4j.Info("serialize");
            return Newtonsoft.Json.JsonConvert.SerializeObject(target);
        }

        public override object JsonDeserialize(string json, Type type)
        {
            log4j.Info("deserialize");
            return Newtonsoft.Json.JsonConvert.DeserializeObject(json, type);
        }
    }
}
