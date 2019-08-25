using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SuperSocket.ProtoBase;
using SocketClient.PackageInfo;

namespace SocketClient.Filter
{
    public class EventSocketFixedHeaderReceiveFilter : FixedHeaderReceiveFilter<PackageInfo.EventPackageInfo>
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public EventSocketFixedHeaderReceiveFilter() : base(8) { }
        public EventSocketFixedHeaderReceiveFilter(int headerSize) : base(headerSize) { }

        public override EventPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            //log4j.Debug("bufferStream.Length: " + bufferStream.Length);
            //log4j.Debug("headerSize: " + this.HeaderSize);
            //log4j.Debug("bodySize: " + ((int)bufferStream.Length - this.HeaderSize));
            //int key1 = bufferStream.Skip(4).ReadByte();
            //int key2 = bufferStream.ReadByte();
            //int key = key2 << 8 | key1;
            //int subKey1 = bufferStream.ReadByte();
            //int subKey2 = bufferStream.ReadByte();
            //int subKey = subKey2 << 8 | subKey1;
            EventPackageInfo ev = new EventPackageInfo(0, 0, "");
            try
            {
                int key = bufferStream.Skip(4).ReadUInt16(true);    // why read little indian??
                int subKey = bufferStream.ReadUInt16(true);

                //log4j.Debug(string.Format("key: {0}, subKey: {1}", key, subKey));

                string body = bufferStream.ReadString((int)bufferStream.Length - this.HeaderSize, Encoding.UTF8);
                ev = new EventPackageInfo(key, subKey, body);
            }
            catch (Exception ex)
            {
                log4j.Error("ResolvePackage: ", ex);
            }
            return ev;
        }

        protected override int GetBodyLengthFromHeader(IBufferStream bufferStream, int length)
        {
            int val1 = bufferStream.ReadUInt16(true);
            int dataSize = bufferStream.ReadUInt16(true);
            // int val3 = bufferStream.ReadUInt16(true);
            // int val4 = bufferStream.ReadUInt16(true);

            dataSize = dataSize - this.HeaderSize;
            //log4j.Debug("val: " + val);
            //if (dataSize > 2000 || val1 != 17408)
            //{
            //    //log4j.Debug("val > 2000 : " + val);
            //    log4j.Debug("incorrect header 17408 or size > 2000");
            //    dataSize = 0;
            //}
            return dataSize;
        }
    }
}
