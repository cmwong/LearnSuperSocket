using System;
using System.Collections.Generic;
using System.Text;

namespace SAEASocket.Custom
{
    public delegate void OnErrorHandler(string sessionID, ushort index, Exception ex);
}
