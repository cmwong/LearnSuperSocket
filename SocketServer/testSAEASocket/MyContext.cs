using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SAEA.Sockets.Base;
using SAEA.Sockets.Interface;
namespace testSAEASocket
{
    class MyContext : IContext
    {
        public IUserToken UserToken { get; set; }
        public IUnpacker Unpacker { get; set; }

        public MyContext()
        {
            MyUnpacker unpacker = new MyUnpacker();
     
            UserToken = new BaseUserToken();
            UserToken.Unpacker = unpacker;
            Unpacker = unpacker;
        }
    }
}
