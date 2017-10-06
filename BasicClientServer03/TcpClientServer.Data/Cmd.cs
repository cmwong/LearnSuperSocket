using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpClientServer.Data
{
    public class Cmd
    {
        public enum TcsCommand
        {
            RequestEcho = 0,
            ResponseEcho = 1,
            RequestAdd = 2,
            ResponseAdd = 3

        }
    }
}
