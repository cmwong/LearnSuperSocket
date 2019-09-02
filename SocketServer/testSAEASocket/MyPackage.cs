using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testSAEASocket
{
    public class MyPackage
    {
        public ushort CMD1 { get; set; }
        // public ushort DataSize { get; set; }
        public ushort MainKey { get; set; }
        public ushort SubKey { get; set; }

        public string Body { get; set; }
    }
}
