using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Linq;

namespace SAEASocket.Custom
{
    /// <summary>
    /// given a sessionID, return an unique number.
    /// same sessionID, if exist, just return the unique number
    /// reuse the number after sessionID was remove from dictionary.
    /// </summary>
    class SessionIDToNumber
    {
        private ushort next = 1;
        private object tLock = new object();
        public int MaxValue { get; private set; } = ushort.MaxValue;

        public SessionIDToNumber() { }
        public SessionIDToNumber(int maxValue)
        {
            MaxValue = maxValue;
        }

        private Stack<int> pool = new Stack<int>();
        private Dictionary<int, string> indexs = new Dictionary<int, string>();
        private Dictionary<string, int> sessionIDs = new Dictionary<string, int>();

        public bool PoolContains(ushort index)
        {
            return pool.Contains(index);
        }
        public bool KeyContains(ushort index)
        {
            return indexs.Keys.Contains(index);
        }
        public bool KeyContains(string sessionID)
        {
            return sessionIDs.Keys.Contains(sessionID);
        }

        internal int GetNext()
        {
            int result = 0;
            if (pool.Count > 0)
            {
                result = pool.Pop();
            }
            else
            {
                result = next++;
                if (next > MaxValue && indexs.Keys.Max() >= MaxValue)
                {
                    throw new OverflowException("No more free number");
                }
            }
            return result;
        }

        /// <summary>
        /// Add a string sessionID and get a unique ushort number
        /// if duplicate sessionID, just return the existing index
        /// throw OverflowException
        ///     when all number 0 to 65534 was in use.
        /// </summary>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        public ushort Add(string sessionID, out bool isAdded)
        {
            isAdded = false;
            ushort index = 0;
            lock (tLock)
            {
                if (sessionIDs.Keys.Contains(sessionID))
                {
                    //throw new Exception("Duplicate sessionID: " + sessionID);
                    index = Get(sessionID);
                }
                else
                {
                    index = (ushort)GetNext();
                    indexs.Add(index, sessionID);
                    sessionIDs.Add(sessionID, index);
                    isAdded = true;
                }
            }

            return index;
        }
        public bool Remove(ushort index)
        {
            bool rVal = false;
            lock (tLock)
            {
                if (indexs.TryGetValue(index, out string sessionID))
                {
                    indexs.Remove(index);
                    sessionIDs.Remove(sessionID);
                    pool.Push(index);

                    rVal = true;
                }
            }
            return rVal;
        }
        public bool Remove(string sessionID)
        {
            bool rVal = false;
            lock (tLock)
            {
                if (sessionIDs.TryGetValue(sessionID, out int index))
                {
                    indexs.Remove(index);
                    sessionIDs.Remove(sessionID);
                    pool.Push(index);

                    rVal = true;
                }
            }
            return rVal;
        }

        public string Get(ushort index)
        {
            indexs.TryGetValue(index, out string sessionID);

            return sessionID;
        }
        public ushort Get(string sessionID)
        {
            sessionIDs.TryGetValue(sessionID, out int index);

            return (ushort)index;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("next: " + next);
            sb.AppendLine("indexs.Count: " + indexs.Count());
            sb.AppendLine("sessionIDs.Count: " + sessionIDs.Count());
            sb.AppendLine("pool.Count: " + pool.Count);

            return sb.ToString();
        }
    }
}
