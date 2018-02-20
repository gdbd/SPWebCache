using System.Threading;
using Microsoft.SharePoint;

namespace SPWebCache.Core
{
    internal class CacheWithLock: Cache
    {
        public CacheWithLock(SPSite site, SPWeb web) : base(site, web)
        {
            Monitor.Enter(site);
            Monitor.Enter(web);
        }

        public override void Dispose()
        {
            Monitor.Exit(Web);
            Monitor.Exit(Site);
        }
    }
}