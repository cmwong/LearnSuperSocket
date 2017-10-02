using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SuperSocket.ProtoBase;
namespace MyClient
{
    /// <summary>
    /// I just need Key and Body
    /// </summary>
    public class StringParser : IStringParser
    {
        private const string SPACE = " ";

        private readonly string m_Spliter;

        /// <summary>
        /// the default singleton instance
        /// </summary>
        public static readonly StringParser DefaultInstance = new StringParser();

        public StringParser() : this(SPACE) { }
        public StringParser(string spliter)
        {
            m_Spliter = spliter;
        }

        public void Parse(string source, out string key, out string body, out string[] parameters)
        {
            int pos = source.IndexOf(m_Spliter);

            if (pos > 0)
            {
                key = source.Substring(0, pos);
                body = source.Substring(pos + 1).Trim();
                parameters = new string[] { };
            }
            else
            {
                key = body = source.Trim();
                parameters = null;
            }
        }
    }
}
