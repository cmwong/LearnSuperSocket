using System;
using System.Collections.Generic;
using System.Text;

namespace SAEASocket.Custom
{
    public class UserToken : SAEA.Sockets.Base.BaseUserToken
    {
        public ushort Index { get; set; }
    }
}
