using System;
using System.Collections.Generic;
using System.Text;

namespace SAEASocket.Custom
{
    public class Package
    {
        public ushort CMD1 { get; set; }
        // public ushort DataSize { get; set; }
        public ushort MainKey { get; set; }
        public ushort SubKey { get; set; }

        public string Body { get; set; }
    }
}
