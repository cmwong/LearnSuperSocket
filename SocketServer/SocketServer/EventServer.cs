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
            this.NewSessionConnected += EventServer_NewSessionConnected;
            this.SessionClosed += EventServer_SessionClosed;
        }

        private void EventServer_SessionClosed(EventSession session, CloseReason value)
        {
            log4j.Info($"sID: {session.SessionID}");
            session.Dispose();
        }

        private void EventServer_NewSessionConnected(EventSession session)
        {
            log4j.Info($"sID: {session.SessionID}");
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
            int count = 50000;

            log4j.Info("TestSendAlot " + count);
            List<EventSession> eventSessions = GetAllSessions().ToList();
            log4j.Info("count: " + eventSessions.Count);

            for (int i = 0; i < count; i++)
            {
                foreach (EventSession eventSession in eventSessions)
                {
                    try
                    {
                        bool isSend = eventSession.Send(key, subKey, data + ": " + i);
                        if(!isSend)
                        {
                            eventSession.Close();
                        }
                        // eventSession.TrySend()
                    } catch(Exception ex)
                    {
                        log4j.Error($"TestSendAlot: {eventSession.SessionID} {data} {i}", ex);
                    }
                }
            }
            log4j.Info("finish");

        }
        public void TestSend10()
        {
            ushort key = 1;
            ushort subKey = 255;
            string data = "this is another message";
            int count = 10;

            log4j.Info("TestSend10 " + count);
            List<EventSession> eventSessions = GetAllSessions().ToList();
            log4j.Info("count: " + eventSessions.Count);

            for (int i = 0; i < count; i++)
            {
                foreach (EventSession eventSession in eventSessions)
                {
                    try
                    {
                        eventSession.Send(key, subKey, data + ": " + i);
                        // eventSession.TrySend()
                    }
                    catch (Exception ex)
                    {
                        log4j.Error($"TestSend10: {eventSession.SessionID} {data} {i}", ex);
                    }
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
