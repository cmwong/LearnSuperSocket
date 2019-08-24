using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer.PackageInfo
{
    public class EventPackageInfo : IRequestInfo
    {
        public string Key { get; protected set; }
        public int MainKey { get; protected set; }
        public int SubKey { get; protected set; }
        public string Body { get; protected set; }

        public EventPackageInfo(int key, int subKey, string body)
        {
            Key = key.ToString() + "_" + subKey.ToString();
            MainKey = key;
            SubKey = subKey;
            Body = body;
        }
    }
}
