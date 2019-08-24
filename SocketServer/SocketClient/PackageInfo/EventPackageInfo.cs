using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketClient.PackageInfo
{
    public class EventPackageInfo : SuperSocket.ProtoBase.IPackageInfo<string>
    {
        public string Key { get; set; }
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
