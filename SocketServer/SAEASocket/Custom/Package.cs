using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SAEASocket.Custom
{
    public class Package
    {
        public ushort CMD1 { get; set; } = 17408;
        // public ushort DataSize { get; set; }
        public ushort MainKey { get; set; }
        public ushort SubKey { get; set; }
        public string Body { get; set; }

        public byte[] ToArray()
        {
            return ToArray(CMD1, MainKey, SubKey, Encoding.UTF8.GetBytes(Body));
        }

        public static byte[] ToArray(ushort cmd, ushort mainCmd, ushort subCmd, byte[] datas)
        {
            //byte[] cmd1 = BitConverter.GetBytes((ushort)17408);
            byte[] cmd1 = BitConverter.GetBytes(cmd);
            byte[] dataSize = new byte[2];
            byte[] cmd3 = BitConverter.GetBytes(mainCmd);
            byte[] cmd4 = BitConverter.GetBytes(subCmd);

            byte[] sendData = cmd1.Concat(dataSize).Concat(cmd3).Concat(cmd4).Concat(datas).ToArray();
            dataSize = BitConverter.GetBytes((ushort)sendData.Length);
            sendData[2] = dataSize[0];
            sendData[3] = dataSize[1];

            return sendData;
        }
    }
}
