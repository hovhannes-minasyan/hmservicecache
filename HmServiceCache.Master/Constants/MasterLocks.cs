using HmServiceCache.Common.Utilities;

namespace HmServiceCache.Master.Constants
{
    internal static class MasterLocks
    {
        public static AsyncReaderWriterLock ConnectionLock = new AsyncReaderWriterLock();
    }
}
