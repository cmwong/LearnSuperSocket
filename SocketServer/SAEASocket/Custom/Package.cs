using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SAEASocket.Custom
{
    public class Package
    {
        public ushort CMD1 { get; set; } = 17408;
        public ushort MainKey { get; set; }
        public ushort SubKey { get; set; }
        public string Body { get; set; }

        public byte[] ToArray()
        {
            return ToArray(CMD1, MainKey, SubKey, ToArray(Body));
        }

        public static byte[] ToArray(string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }
        public static byte[] ToArray(ushort cmd, ushort mainCmd, ushort subCmd, byte[] datas)
        {
            byte[] cmd1 = BitConverter.GetBytes(cmd);
            byte[] dataSize = BitConverter.GetBytes((ushort)(datas.Length + Unpacker.HeaderSize));
            byte[] cmd3 = BitConverter.GetBytes(mainCmd);
            byte[] cmd4 = BitConverter.GetBytes(subCmd);

            return cmd1.Concat(dataSize).Concat(cmd3).Concat(cmd4).Concat(datas).ToArray();
        }
    }
}
