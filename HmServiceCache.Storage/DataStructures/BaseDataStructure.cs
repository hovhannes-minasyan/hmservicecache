using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HmServiceCache.Common.Extensions;

namespace HmServiceCache.Storage.DataStructures
{
    public class BaseDataStructure<T>
    {
        private readonly ConcurrentDictionary<string, long> lastTimeStamps = new ConcurrentDictionary<string, long>();
        private readonly ConcurrentDictionary<string, ReaderWriterLock> lockObjects = new ConcurrentDictionary<string, ReaderWriterLock>();
        protected readonly ConcurrentDictionary<string, T> collection = new ConcurrentDictionary<string, T>();

        protected bool UpdateData(Func<bool> operation, long timeStamp, string key)
        {
            ReaderWriterLock lockObj;
            lock (lockObjects)
            {
                lockObj = lockObjects.SafeKey(key);
            }

            lockObj.AcquireWriterLock(TimeSpan.FromSeconds(30));

            try
            {
                if (lastTimeStamps.GetValueOrDefault(key) > timeStamp)
                {
                    return false;
                }

                var result = operation();
                lastTimeStamps[key] = timeStamp;
                return result;
            }
            finally
            {
                lockObj.ReleaseWriterLock();
            }
        }

        protected void UpdateData(Action operation, long timeStamp, string key) => UpdateData(() => { operation(); return true; }, timeStamp, key);

        public void RemoveKey(string key, long timeStamp) => UpdateData(() =>
        {
            collection.Remove(key, out _);
            lockObjects.TryRemove(key, out _);
            lastTimeStamps.TryRemove(key, out _);
        }, timeStamp, key);

        public T GetByKey(string key) => GetSomeData(() => collection.GetValueOrDefault(key), key);

        public TData GetSomeData<TData>(Func<TData> operation, string key)
        {
            ReaderWriterLock lockObj;
            lock (lockObjects)
            {
                lockObj = lockObjects.SafeKey(key);
            }

            lockObj.AcquireReaderLock(TimeSpan.FromSeconds(30));

            try
            {
                return operation();
            }
            finally
            {
                lockObj.ReleaseReaderLock();
            }
        }

        public void Empty()
        {
            collection.Clear();
            lockObjects.Clear();
            lastTimeStamps.Clear();
        }

        public Dictionary<string, T> GetAll()
        {
            return collection.ToDictionary(a => a.Key, a => a.Value);
        }

        public void Set(Dictionary<string, T> data)
        {
            foreach (var pair in data)
            {
                collection.TryAdd(pair.Key, pair.Value);
            }
        }
    }
}
