using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocketServer.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer.Filter.Tests
{
    [TestClass()]
    public class EventSocketFixedHeaderReceiveFilterTests
    {
        [TestMethod()]
        public void TestGetBodyLengthFromHeaderTest2()
        {
            // https://code.msdn.microsoft.com/Unit-Testing-Private-01117912

            //byte[] cmd1 = BitConverter.GetBytes((ushort)17408);
            //byte[] dataSize = new byte[2];
            //byte[] cmd3 = BitConverter.GetBytes((ushort)1);
            //byte[] cmd4 = BitConverter.GetBytes((ushort)1);
            //byte[] datas = Encoding.UTF8.GetBytes("abcde测试测试");


            //byte[] sendData = cmd1.Concat(dataSize).Concat(cmd3).Concat(cmd4).Concat(datas).ToArray();
            //dataSize = BitConverter.GetBytes((ushort)sendData.Length);
            //sendData[2] = dataSize[0];
            //sendData[3] = dataSize[1];

            byte[] datas = Encoding.UTF8.GetBytes("abcde测试测试");
            byte[] cmd1 = BitConverter.GetBytes((ushort)17408);
            byte[] dataSize = BitConverter.GetBytes((ushort)(datas.Length + 8));
            byte[] cmd3 = BitConverter.GetBytes((ushort)1);
            byte[] cmd4 = BitConverter.GetBytes((ushort)1);

            byte[] sendData = cmd1.Concat(dataSize).Concat(cmd3).Concat(cmd4).Concat(datas).ToArray();

            EventSocketFixedHeaderReceiveFilter eFilter = new EventSocketFixedHeaderReceiveFilter();
            PrivateObject obj = new PrivateObject(new EventSocketFixedHeaderReceiveFilter());

            int actualBodyLength = (int)obj.Invoke("GetBodyLengthFromHeader", sendData, 0, sendData.Length);
            int expected = sendData.Length - 8;

            Assert.AreEqual(expected, actualBodyLength, expected + " : " + actualBodyLength);
        }
    }
}