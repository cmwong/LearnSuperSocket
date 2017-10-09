using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSocket.Data
{
    public class RequestAdd : BaseData
    {
        public List<int> Param { get; set; }

        public RequestAdd()
        {
            Param = new List<int>();
        }
    }
}
