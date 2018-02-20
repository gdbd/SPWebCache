using System;
using System.Linq;
using System.Security.Principal;
using Microsoft.SharePoint;
using SPWebCache.Common;

namespace SPWebCache.Core
{
    internal class ByUrlRequest : Request
    {
        private string _webUrlOfRequest;
        public string WebUrl;

        public override string RequestKey { get { return WebUrl; } }

        public override string SiteUrlOfRequest()
        {
            return SiteMap.Sites
                .Where(su => WebUrl.ToLower().Contains(su.Url.ToLower()))
                .OrderBy(su => su.Url.Length)
                .Select(h => h.Url)
                .LastOrDefault();
        }

        public override string WebUrlOfRequest()
        {
            return _webUrlOfRequest ?? (_webUrlOfRequest = SiteMap.Webs
                .Where(wu => WebUrl.ToLower().Contains(wu.Url.ToLower()))
                .OrderBy(wu => wu.Url.Length)
                .Select(h => h.Url)
                .LastOrDefault());
        }

        public override SPSite OpenNewSite()
        {
            if (Identity.IsAnonymous)
            {
                return new SPSite(WebUrl, SPUserToken.SystemAccount);
            }
            else
            {
                using (new ImpersonatedScope(Identity))
                {
                    return new SPSite(WebUrl);
                }
            }
        }

        public override SPWeb OpenNewWeb(SPSite site)
        {
            var siteRelativeUrl = site.ServerRelativeUrl + WebUrl.Replace(site.Url, string.Empty);

            if (Identity.IsAnonymous)
            {
                return site.OpenWeb();
            }
            else
            {
                using (new ImpersonatedScope(Identity))
                {
                    return site.OpenWeb(siteRelativeUrl, false);
                }
            }
        }

        public override bool IsCurrentWebRequest()
        {
            return SPContext.Current != null && Identity != WindowsIdentity.GetAnonymous() &&
                   WebUrl.StartsWith(_webUrlOfRequest, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}