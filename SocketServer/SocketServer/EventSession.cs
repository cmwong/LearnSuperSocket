using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketServer.PackageInfo;
using SuperSocket.SocketBase;

namespace SocketServer
{
    public class EventSession : SuperSocket.SocketBase.AppSession<EventSession, PackageInfo.EventPackageInfo>
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected override void OnSessionStarted()
        {
            log4j.Info("session start " + this.SessionID);
        }
        protected override void OnSessionClosed(CloseReason reason)
        {
            log4j.Info("session closed " + reason.ToString());
            base.OnSessionClosed(reason);
        }
        protected override void HandleUnknownRequest(EventPackageInfo requestInfo)
        {
            log4j.Info($"unknow request k: {requestInfo.MainKey}, sk: {requestInfo.SubKey}, b: {requestInfo.Body}");
        }
    }
}
