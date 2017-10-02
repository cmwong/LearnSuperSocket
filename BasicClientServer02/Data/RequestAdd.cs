using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
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
