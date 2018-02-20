using System;
using System.Linq;

namespace SPWebCache.Core
{
    internal class SiteMapImpl : ISiteMap
    {
        public SiteMapImpl()
        {
            SiteMap.Build();
            SiteMap.WaitForBuild();
        }

        public Guid GetSiteId(string url)
        {
            var site = SiteMap.Sites
                .Where(su => url.ToLower().Contains(su.Url.ToLower()))
                .OrderBy(su => su.Url.Length)
                .LastOrDefault();

            if (site == null)
            {
                throw new Exception(string.Format("Cannot find SPSite Id by url: {0}", url));
            }

            return site.Id;
        }

        public Guid GetWebId(string url)
        {
            var web = SiteMap.Webs
                .Where(wu => url.ToLower().Contains(wu.Url.ToLower()))
                .OrderBy(wu => wu.Url.Length)
                .LastOrDefault();

            if (web == null)
            {
                throw new Exception(string.Format("Cannot find SPWeb Id by url: {0}", url));
            }

            return web.Id;
        }

        public string GetSiteUrl(Guid siteId)
        {
            var site = SiteMap.Sites
                .Where(su => su.Id == siteId)
                .OrderBy(su => su.Url.Length)
                .LastOrDefault();

            if (site == null)
            {
                throw new Exception(string.Format("Cannot find SPSite URL by ID: {0}", siteId));
            }

            return site.Url;
        }

        public string GetWebUrl(Guid webId)
        {
            var web = SiteMap.Webs
                .Where(wu => wu.Id == webId)
                .OrderBy(wu => wu.Url.Length)
                .LastOrDefault();

            if (web == null)
            {
                throw new Exception(string.Format("Cannot find SPWeb URL by ID: {0}", webId));
            }

            return web.Url;
        }
    }
}