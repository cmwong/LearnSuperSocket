using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SAEA.Sockets.Interface;
namespace testSAEASocket
{
    class MyUnpacker : IUnpacker
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public const int HeaderSize = 8;

        List<byte> _cache = new List<byte>();
        public void Clear()
        {
            _cache.Clear();
        }

        public void Unpack(byte[] data, Action<ISocketProtocal> unpackCallback, Action<DateTime> onHeart = null, Action<byte[]> onFile = null)
        {
            _cache.AddRange(data);
        }
        public void Unpack(byte[] data, Action<MyPackage> unpackCallBack)
        {
            _cache.AddRange(data);
            // log4j.Info("_cache length: " + _cache.Count);
            while (_cache.Count >= HeaderSize)
            {
                byte[] buffer = _cache.ToArray();
                int messageLength = GetLength(buffer);
                // log4j.Info($"buffer.length: {buffer.Length}, messageLength: {messageLength}");
                if (buffer.Length >= messageLength)
                {
                    MyPackage myPackage = GetPackage(buffer, messageLength);
                    _cache.RemoveRange(0, messageLength);
                    unpackCallBack?.Invoke(myPackage);
                }
                else
                {
                    return;
                }
            }
        }
        public int GetLength(byte[] data)
        {
            int dataSize = BitConverter.ToUInt16(data, 2);
            // dataSize = dataSize - HeaderSize;

            return dataSize;
        }
        public MyPackage GetPackage(byte[] buffer, int length)
        {
            MyPackage myPackage = new MyPackage();
            myPackage.CMD1 = BitConverter.ToUInt16(buffer, 0);
            myPackage.MainKey = BitConverter.ToUInt16(buffer, 4);
            myPackage.SubKey = BitConverter.ToUInt16(buffer, 6);
            myPackage.Body = Encoding.UTF8.GetString(buffer, HeaderSize, length - HeaderSize);

            return myPackage;
        }
    }
}
