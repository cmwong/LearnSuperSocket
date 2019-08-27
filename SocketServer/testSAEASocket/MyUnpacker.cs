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
        List<byte> _cache = new List<byte>();
        public void Clear()
        {
            _cache.Clear();
        }

        public void Unpack(byte[] data, Action<ISocketProtocal> unpackCallback, Action<DateTime> onHeart = null, Action<byte[]> onFile = null)
        {
            _cache.AddRange(data);
        }
    }
}
