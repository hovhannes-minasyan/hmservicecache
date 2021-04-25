﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HmServiceCache.Storage.Interfaces
{
    public interface IDataStorageReader
    {
        object GetValue(string key);

        List<object> GetList(string key);
        object GetListValue(string key, int index);

        Dictionary<string, object> GetHasMap(string key);
        object GetHashValue(string key, string hashKey);
    }
}
