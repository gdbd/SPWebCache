using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SPWebCache.Common;
using SPWebCache.Core;

namespace SPWebCache.Layouts.SPWebCache
{
    public partial class Status : LayoutsPageBase
    {
        public List<Row> Sites;
        public List<Row> Webs;

        protected void Page_Load(object sender, EventArgs e)
        {
            Sites = RequestQueue.Instance.WebFactory.Sites.Select(s => new Row(s.Value)).ToList();
            Webs = RequestQueue.Instance.WebFactory.Webs.Select(w => new Row(w.Value)).ToList();
        }


        public class Row
        {

            internal Row(CacheEntity<SPWeb> web)
            {
                Url = web.CachedObject.Url;
                Id = web.CachedObject.ID;
                User = web.CachedObject.CurrentUser.LoginName;
                LastUsed = (int) (DateTime.Now - web.LastUsed).TotalSeconds;

                bool entered = false;
                try
                {
                    entered = Monitor.TryEnter(web.CachedObject);
                    Locked = !entered;
                }
                finally
                {
                    if (entered) Monitor.Exit(web.CachedObject);
                }
            }

            internal Row(CacheEntity<SPSite> site)
            {
                Url = site.CachedObject.Url;
                Id = site.CachedObject.ID;
                User = site.User.IsAuthenticated ? site.User.Name : "SHAREPOINT\\system";
                LastUsed = (int) (DateTime.Now - site.LastUsed).TotalSeconds;

                bool entered = false;
                try
                {
                    entered = Monitor.TryEnter(site.CachedObject);
                    Locked = !entered;
                }
                finally
                {
                    if (entered) Monitor.Exit(site.CachedObject);
                }
            }


            public Guid Id { get; set; }

            public string Url { get; set; }

            public string User { get; set; }

            public int LastUsed { get; set; }

            public bool Locked { get; set; }
        }
    }
}