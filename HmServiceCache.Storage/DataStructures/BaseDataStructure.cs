using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace HmServiceCache.Storage.DataStructures
{
    public class BaseDataStructure<T>
    {
        private readonly ConcurrentDictionary<string, long> lastTimeStamps = new ConcurrentDictionary<string, long>();
        private readonly ConcurrentDictionary<string, object> lockObjects = new ConcurrentDictionary<string, object>();
        protected readonly ConcurrentDictionary<string, T> collection = new ConcurrentDictionary<string, T>();

        protected bool UpdateData(Func<bool> operation, long timeStamp, string key)
        {
            object lockObj;
            lock (lockObjects)
            {
                if (!lockObjects.ContainsKey(key))
                {
                    lockObjects[key] = new object();
                }
                lockObj = lockObjects[key];
            }

            lock (lockObj)
            {
                if (lastTimeStamps.GetValueOrDefault(key) > timeStamp)
                {
                    return false;
                }

                var result = operation();
                lastTimeStamps[key] = timeStamp;
                return result;
            }
        }

        protected void UpdateData(Action operation, long timeStamp, string key)
        {
            var result = UpdateData(() => { operation(); return true; }, timeStamp, key);
        }

        public void RemoveKey(string key, long timeStamp)
        {
            UpdateData(() => collection.Remove(key, out _), timeStamp, key);
        }

        public T GetByKey(string key) 
        {
            return collection.GetValueOrDefault(key);
        }

        public void Empty() 
        {
            collection.Clear();
            lockObjects.Clear();
            lastTimeStamps.Clear();
        }
    }
}
