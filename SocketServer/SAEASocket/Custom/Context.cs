using System;
using System.Collections.Generic;
using System.Text;

using SAEA.Sockets.Base;
using SAEA.Sockets.Interface;

namespace SAEASocket.Custom
{
    class Context : IContext
    {
        public IUserToken UserToken { get; set; }
        public IUnpacker Unpacker { get; set; }

        public Context()
        {
            Unpacker unpacker = new Unpacker();

            UserToken = new BaseUserToken();
            UserToken.Unpacker = unpacker;
            Unpacker = unpacker;
        }
    }
}
