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

        public bool Send(ushort mainCmd, ushort subCmd, string dataText)
        {
            byte[] datas = Encoding.UTF8.GetBytes(dataText);

            return Send(mainCmd, subCmd, datas);
        }

        public bool Send(ushort mainCmd, ushort subCmd, byte[] datas)
        {
            bool val = false;
            if (!Connected)
                return val;

            byte[] cmd1 = BitConverter.GetBytes((ushort)17408);
            byte[] dataSize = new byte[2];
            byte[] cmd3 = BitConverter.GetBytes(mainCmd);
            byte[] cmd4 = BitConverter.GetBytes(subCmd);

            //log4j.Info("cmd1: " + BitConverter.ToString(cmd1));
            //log4j.Info("cmd3: " + BitConverter.ToString(cmd3));
            //log4j.Info("cmd4: " + BitConverter.ToString(cmd4));

            byte[] sendData = cmd1.Concat(dataSize).Concat(cmd3).Concat(cmd4).Concat(datas).ToArray();
            dataSize = BitConverter.GetBytes((ushort)sendData.Length);
            sendData[2] = dataSize[0];
            sendData[3] = dataSize[1];

            if (datas.Length > AppServer.Config.MaxRequestLength)
            {
                log4j.Debug("data too long");
                return val;
            }

            ArraySegment<byte> segment = new ArraySegment<byte>(sendData);
            //Send(segment);
            //val = true;
            //return val;

            return TrySend(segment);

        }
    }
}
