using System;
using System.Collections.Generic;
using System.Text;

namespace SAEASocket.Custom
{
    public delegate void OnDisconnectedHandler(string sessionID, ushort index, Exception ex);
}
