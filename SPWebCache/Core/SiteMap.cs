using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using SPWebCache.Common;

namespace SPWebCache.Core
{
    internal static class SiteMap
    {
        private static EventWaitHandle _waitHandle = new ManualResetEvent(false);
        private static bool _isBuilt;
        internal static List<HashEntity> Sites = new List<HashEntity>();
        internal static List<HashEntity> Webs = new List<HashEntity>();
        
        internal static void Build()
        {
            if (!_isBuilt)
            {
                _isBuilt = true;
                var threadBuildUrls =  new Thread(() => SPSecurity.RunWithElevatedPrivileges(BuildsUrlMap));
                threadBuildUrls.Start();
            }
        }

        internal static void WaitForBuild()
        {
            if (_waitHandle != null)
            {
                lock (_waitHandle)
                {
                    if (_waitHandle != null)
                    {
                        _waitHandle.WaitOne();
                        _waitHandle.Close();
                        _waitHandle = null;
                    }
                }
            }
        }

        private static void BuildsUrlMap()
        {
            var apps = SPFarm.Local.Services.OfType<SPWebService>()
                   .SelectMany(ws => ws.WebApplications)
                   .Where(app => !app.IsAdministrationWebApplication)
                   .ToList();
            foreach (var webApplication in apps)
            {
                foreach (SPSite site in webApplication.Sites)
                {
                    try
                    {
                        Sites.Add(new HashEntity(site.Url, site.ID));

                        foreach (SPWeb web in site.AllWebs)
                        {
                            try
                            {
                                Webs.Add(new HashEntity(web.Url, web.ID));
                            }
                            finally
                            {
                                web.Dispose();
                            }
                        }
                    }
                    finally
                    {
                        site.Dispose();
                    }
                }
            }
            _waitHandle.Set();
        }
    }
}
