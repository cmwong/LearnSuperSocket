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
            if (requestInfo.MainKey == 1 && requestInfo.SubKey == 2)
            {
                Task.Run(() =>
                {
                    TestSendAlot(session);
                });
            }
        }

        public void TestSendAlot()
        {
            ushort key = 1;
            ushort subKey = 255;
            string data = "this is some text 哈哈 ";
            int count = 10000;

            log4j.Info("TestSendAlot " + count);
            for (int i = 0; i < count; i++)
            {
                foreach (EventSession eventSession in GetAllSessions())
                {
                    eventSession.Send(key, subKey, data + ": " + i);
                }
            }
            log4j.Info("finish");

        }

        private void TestSendAlot(EventSession session)
        {
            ushort key = 1;
            ushort subKey = 255;
            string data = "only send to single client 哈哈 ";
            int count = 10000;

            log4j.Info("TestSendAlot " + count);
            for (int i = 0; i < count; i++)
            {
                session.Send(key, subKey, data + ": " + i);
            }
            log4j.Info("finish");
        }
    }
}
