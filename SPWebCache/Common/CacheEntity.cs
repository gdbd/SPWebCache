using System;
using System.Security.Principal;

namespace SPWebCache.Common
{
    internal class CacheEntity<T>
    {
        public T CachedObject;
        public DateTime LastUsed;
        public IIdentity User;
    }
}
