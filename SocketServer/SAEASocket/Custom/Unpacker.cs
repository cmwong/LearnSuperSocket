﻿using System;
using System.Collections.Generic;
using System.Text;

using SAEA.Sockets.Interface;

namespace SAEASocket.Custom
{
    /// <summary>
    /// Header size is 8 bytes, store 4 data (each data is 2 byte(ushort))
    /// CMD1        ushort
    /// DataSize    ushort
    /// MainCmd     ushort
    /// SubCmd      ushort
    /// </summary>
    public class Unpacker : IUnpacker
    {
        public static int HeaderSize = 8;
        List<byte> _cache = new List<byte>();
        public void Clear()
        {
            _cache.Clear();
        }

        public void Unpack(byte[] data, Action<ISocketProtocal> unpackCallback, Action<DateTime> onHeart = null, Action<byte[]> onFile = null)
        {
            throw new NotImplementedException();
        }
        public void Unpack(byte[] data, Action<Package> unpackCallBack)
        {
            _cache.AddRange(data);
            while (_cache.Count >= HeaderSize)
            {
                byte[] buffer = _cache.ToArray();
                int messageLength = GetLength(buffer);
                if (buffer.Length >= messageLength)
                {
                    Package myPackage = GetPackage(buffer, messageLength);
                    _cache.RemoveRange(0, messageLength);
                    unpackCallBack?.Invoke(myPackage);
                }
                else
                {
                    return;
                }
            }
        }
        private int GetLength(byte[] data)
        {
            // the size store in the header is icluding the header size
            int dataSize = BitConverter.ToUInt16(data, 2);

            return dataSize;
        }
        private Package GetPackage(byte[] buffer, int length)
        {
            Package package = new Package();
            package.CMD1 = BitConverter.ToUInt16(buffer, 0);
            package.MainKey = BitConverter.ToUInt16(buffer, 4);
            package.SubKey = BitConverter.ToUInt16(buffer, 6);
            package.Body = Encoding.UTF8.GetString(buffer, HeaderSize, length - HeaderSize);

            return package;
        }
    }
}
