using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketServer.PackageInfo;
using SuperSocket.SocketBase;

namespace SocketServer
{
    public class EventServer : SuperSocket.SocketBase.AppServer<EventSession, PackageInfo.EventPackageInfo>
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public EventServer() :
            base(new SuperSocket.SocketBase.Protocol.DefaultReceiveFilterFactory<Filter.EventSocketFixedHeaderReceiveFilter, PackageInfo.EventPackageInfo>())
        {
            Init();
        }

        private void Init()
        {
            this.NewRequestReceived += EventServer_NewRequestReceived;
        }

        private void EventServer_NewRequestReceived(EventSession session, EventPackageInfo requestInfo)
        {
            log4j.Info($"sID: {session.SessionID}, k: {requestInfo.MainKey}, sk: {requestInfo.SubKey}, b: {requestInfo.Body}");
        }
    }
}
