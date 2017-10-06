using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
namespace TcpClientServer.Server
{
    public class StringSession : AppSession<StringSession>
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected override void OnSessionStarted()
        {
            base.OnSessionStarted();
            log4j.Info(string.Format("SessionStarted {0}", this.RemoteEndPoint));
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            base.OnSessionClosed(reason);
            log4j.Info(string.Format("SessionClosed {0}", this.RemoteEndPoint));
        }

        protected override void HandleException(Exception e)
        {
            log4j.Info("Application error", e);
        }

        protected override void HandleUnknownRequest(StringRequestInfo requestInfo)
        {
            log4j.Info("Unknow request: " + requestInfo.Key);
        }
    }
}
