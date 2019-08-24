using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketServer.PackageInfo;

namespace SocketServer.Filter
{
    public class EventSocketFixedHeaderReceiveFilter : SuperSocket.Facility.Protocol.FixedHeaderReceiveFilter<PackageInfo.EventPackageInfo>
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public EventSocketFixedHeaderReceiveFilter() : base(8) { }
        // public EventSocketFixedHeaderReceiveFilter(int headerSize) : base(headerSize) { }

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            int val1 = BitConverter.ToUInt16(header, offset);
            int dataSize = BitConverter.ToUInt16(header, offset + 2);
            int val3 = BitConverter.ToUInt16(header, offset + 4);
            int val4 = BitConverter.ToUInt16(header, offset + 6);
            dataSize = dataSize - this.Size;
            // if (dataSize > 2000 || val1 != 17408)
            if(dataSize > 2000)
            {
                //log4j.Debug("val > 2000 : " + val);
                //log4j.Debug("incorrect header 17408 or size > 2000");
                log4j.Debug("dataSize > 2000");
                dataSize = 0;
            }
            return dataSize;
        }

        protected override EventPackageInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            EventPackageInfo ev = new EventPackageInfo(0, 0, "");
            try
            {
                //byte[] headers = header.ToArray();
                int key = BitConverter.ToUInt16(header.Array, header.Offset + 4);
                int subKey = BitConverter.ToUInt16(header.Array, header.Offset + 6);
                string body = Encoding.UTF8.GetString(bodyBuffer, offset, length);
                ev = new EventPackageInfo(key, subKey, body);
            } catch(Exception ex)
            {
                log4j.Error("ResolvePackage: ", ex);
            }
            return ev;
        }
    }
}
