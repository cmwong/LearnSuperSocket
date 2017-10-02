using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SuperSocket.ProtoBase;
namespace MyClient
{
    public class StringReceiveFilter : TerminatorReceiveFilter<StringPackageInfo>
    {
        private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public StringReceiveFilter() : base(Encoding.UTF8.GetBytes("\n"))
        {

        }

        public override StringPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            string source = bufferStream.ReadString((int)bufferStream.Length, Encoding.UTF8);

            //return new StringPackageInfo(source, BasicStringParser.DefaultInstance);
            return new StringPackageInfo(source, StringParser.DefaultInstance);
        }

    }
}
