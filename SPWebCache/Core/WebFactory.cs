using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.SharePoint;
using SPWebCache.Common;

namespace SPWebCache.Core
{
    internal class WebFactory : IDisposable
    {
        const int LifeTimeInMinutes = 30;
        const int CheckPeriodMiliseconds = 5 * 60 * 1000;

        internal Dictionary<int, CacheEntity<SPSite>> Sites = new Dictionary<int, CacheEntity<SPSite>>();
        internal Dictionary<int, CacheEntity<SPWeb>> Webs = new Dictionary<int, CacheEntity<SPWeb>>();
        
        public void Dispose()
        {
            lock (Webs)
            {
                var idsToRemove = Webs.Select(w => w.Key).ToList();
                foreach (var id in idsToRemove)
                {
                    var web = Webs[id];
                    
                    web.CachedObject.Dispose();
                    Webs.Remove(id);
                }
            }

            lock (Sites)
            {
                var idsToRemove = Sites.Select(w => w.Key).ToList();
                foreach (var id in idsToRemove)
                {
                    var site = Sites[id];
                    site.CachedObject.Dispose();
                    Sites.Remove(id);
                }
            }
        }

        internal void ProccessRequest(Request request)
        {
            var siteOfUrl = request.SiteUrlOfRequest();
            var webOfUrl = request.WebUrlOfRequest();

            if (siteOfUrl == null || webOfUrl == null)
            {
                var ex = new Exception(string.Format("Cannot find site or web in farm by request: '{0}'", request.RequestKey));
                request.CompleteCallback(null, null, ex);
                return;
            }
            
            if (request.IsCurrentWebRequest())
            {
                request.CompleteCallback(SPContext.Current.Site, SPContext.Current.Web, null);
                return;
            }

            var siteHash = siteOfUrl.GetHashCode() + request.Identity.Name.GetHashCode();
            var webHash = webOfUrl.GetHashCode() + request.Identity.Name.GetHashCode();

            if (request.RequestHash != null)
            {
                siteHash += request.RequestHash.Value;
                webHash += request.RequestHash.Value;
            }

            CacheEntity<SPSite> site;
            lock (Sites)
            {
                if (!Sites.TryGetValue(siteHash, out site))
                {
                    site = new CacheEntity<SPSite>
                    {
                        CachedObject = request.OpenNewSite(), 
                        User = request.Identity,
                    };
 
                    // необходимо для инициализации SPRequest внутри SPSite
                    var url = site.CachedObject.AllowUnsafeUpdates;

                    Sites.Add(siteHash, site);
                }
            }

            CacheEntity<SPWeb> web;
            lock (Webs)
            {
                if (!Webs.TryGetValue(webHash, out web))
                {
                    web = new CacheEntity<SPWeb>
                    {
                        CachedObject = request.OpenNewWeb(site.CachedObject),
                        User = request.Identity,
                    };
                    
                    // необходимо для инициализации SPRequest внутри SPWeb
                    var url = web.CachedObject.Url;

                    Webs.Add(webHash, web);
                }
            }

            site.LastUsed = web.LastUsed = DateTime.Now;
            request.CompleteCallback(site.CachedObject, web.CachedObject, null);
        }

        internal void DisposeLongNotUsed()
        {
            while ((Thread.CurrentThread.ThreadState != ThreadState.AbortRequested) && (Thread.CurrentThread.ThreadState != ThreadState.StopRequested) && (Thread.CurrentThread.ThreadState != ThreadState.SuspendRequested))
            {
                Thread.Sleep(CheckPeriodMiliseconds);

                var idsToRemove = new List<int>();
                lock (Webs)
                {
                    foreach (var web in Webs)
                    {
                        var timeFromLastUsed = DateTime.Now - web.Value.LastUsed;
                        if (timeFromLastUsed.Minutes >= LifeTimeInMinutes)
                        {
                            web.Value.CachedObject.Dispose();
                            idsToRemove.Add(web.Key);
                        }
                    }
                
                    foreach (var i in idsToRemove)
                    {
                        Webs.Remove(i);
                    }
                }

                idsToRemove.Clear();

                lock (Sites)
                {
                    foreach (var site in Sites)
                    {
                        var timeFromLastUsed = DateTime.Now - site.Value.LastUsed;
                        if (timeFromLastUsed.Minutes >= LifeTimeInMinutes)
                        {
                            site.Value.CachedObject.Dispose();
                            idsToRemove.Add(site.Key);
                        }
                    }
                
                    foreach (var i in idsToRemove)
                    {
                        Sites.Remove(i);
                    }
                }
            }
        }	    
    }
}
