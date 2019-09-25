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
        private ushort next = 0;
        private object tLock = new object();

        private ConcurrentDictionary<ushort, string> indexKeys = new ConcurrentDictionary<ushort, string>();
        private ConcurrentDictionary<string, ushort> sessionKeys = new ConcurrentDictionary<string, ushort>();
        private ConcurrentBag<ushort> pool = new ConcurrentBag<ushort>();

        private ushort GetNext()
        {
            ushort result = 0;
            if (pool.Count > 0)
            {
                if (pool.TryTake(out result))
                {
                    return result;
                }
                else
                {
                    return GetNext();
                }
            }
            else
            {
                lock (tLock)
                {
                    result = next++;
                    if (next >= ushort.MaxValue && indexKeys.Keys.Count() >= ushort.MaxValue)
                    {
                        throw new OverflowException("No more free number");
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// throw OverflowException
        /// when all number 0 to 65534 was in use.
        /// </summary>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        public ushort Add(string sessionID)
        {
            ushort index = 0;
            if (sessionKeys.ContainsKey(sessionID))
            {
                sessionKeys.TryGetValue(sessionID, out index);
                return index;
            }
            index = GetNext();
            indexKeys.TryAdd(index, sessionID);
            sessionKeys.TryAdd(sessionID, index);

            return index;
        }
        public void Remove(ushort index)
        {
            if (indexKeys.TryRemove(index, out string sessionID))
            {
                sessionKeys.TryRemove(sessionID, out ushort _index);
                pool.Add(index);
            }
        }
        public void Remove(string sessionID)
        {
            Remove(Get(sessionID));
        }

        public string Get(ushort index)
        {
            indexKeys.TryGetValue(index, out string sessionID);

            return sessionID;
        }
        public ushort Get(string sessionID)
        {
            sessionKeys.TryGetValue(sessionID, out ushort index);

            return index;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("next: " + next);
            sb.AppendLine("indexKeys.Count: " + indexKeys.Keys.Count());
            sb.AppendLine("sessionKeys.Count: " + sessionKeys.Keys.Count());
            sb.AppendLine("pool.Count: " + pool.Count);

            return sb.ToString();
        }
    }
}
