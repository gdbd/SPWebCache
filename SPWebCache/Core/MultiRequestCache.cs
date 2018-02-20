using System;
using System.Security.Principal;
using System.Threading;
using Microsoft.SharePoint;

namespace SPWebCache.Core
{
	public class MultiRequestCache : IWebCache
	{
	    private static string _appPoolAccount;

        public Cache GetWeb(string url)
		{
            var req = new ByUrlRequest
                {
                    Identity = WindowsIdentity.GetCurrent(),
                    WebUrl = url,
                };
            return GetWebCore(req);
		}

        public Cache GetElevatedWeb(string url)
		{
            var req = new ByUrlRequest
            {
                Identity = WindowsIdentity.GetAnonymous(),
                WebUrl = url,
            };
            return GetWebCore(req);
		}

        public Cache GetWeb(Guid siteId, Guid webId)
	    {
            var req = new ByGuidRequest
            {
                Identity = WindowsIdentity.GetCurrent(),
                SiteId = siteId,
                WebId = webId,
            };
            return GetWebCore(req);
	    }

        public Cache GetElevatedWeb(Guid siteId, Guid webId)
        {
            var req = new ByGuidRequest
            {
                Identity = WindowsIdentity.GetAnonymous(),
                SiteId = siteId,
                WebId = webId,
            };
            return GetWebCore(req);
	    }

        public virtual void Dispose()
        {
        }

        internal virtual Cache GetWebCore(Request request)
        {
            if (IsRunedUnderElevetedPrivilegies(request))
            {
                request.Identity = WindowsIdentity.GetAnonymous();
            }

            SPSite site = null;
            SPWeb web = null;
            Exception ex = null;
            var wh = new ManualResetEvent(false);

            request.CompleteCallback = (s, w, e) =>
            {
                site = s;
                web = w;
                ex = e;
                wh.Set();
            };
            try
            {
                RequestQueue.Instance.QueueRequest(request);
                wh.WaitOne();
            }
            finally
            {
                wh.Close();
            }

            if (ex != null) throw ex;
            return new CacheWithLock(site, web);
        }
	   

        internal bool IsRunedUnderElevetedPrivilegies(Request request)
        {
            if (_appPoolAccount == null)
            {
                SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    var windowsIdentity = WindowsIdentity.GetCurrent();
                    if (windowsIdentity != null) _appPoolAccount = windowsIdentity.Name;
                });
            }

            return _appPoolAccount != null && _appPoolAccount.Equals(request.Identity.Name, StringComparison.InvariantCultureIgnoreCase);
        }
	}
}
