using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Linq;

namespace SAEASocket.Custom
{
    /// <summary>
    /// given a sessionID, return an unique number.
    /// same sessionID, if exist, will return same number.
    /// reuse the number after sessionID was remove from dictionary.
    /// minValue = 0
    /// maxValue = 65534
    /// </summary>
    class SessionIDToNumber
    {
        // private static readonly log4net.ILog log4j = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
        /// throw OverflowException
        ///     when all number 0 to 65534 was in use.
        /// throw Exception 
        ///     duplicate sessionID
        /// </summary>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        public ushort Add(string sessionID)
        {
            //log4j.Debug("in sID: " + sessionID);
            ushort index = 0;
            lock (tLock)
            {
                if (sessionIDs.Keys.Contains(sessionID))
                {
                    throw new Exception("Duplicate sessionID: " + sessionID);
                }
                else
                {
                    index = (ushort)GetNext();
                    indexs.Add(index, sessionID);
                    sessionIDs.Add(sessionID, index);
                }
            }
            //log4j.Debug("out sID: " + sessionID + ", index: " + index);

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
